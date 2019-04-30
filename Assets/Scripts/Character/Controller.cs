using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// codes for modulated audio getter
public enum SoundDriverCode { Speed, Thrust, Hover, ForceField }
// acceleration modes
public enum DriveMode { Accelerate, Decelerate, Neutral }


// class attached to player (or other racer) responsible for applying banking, turning, and acceleration to model and rigidbody
public class Controller : MonoBehaviour
{
	float thrusterMass;
	float axialThrust;                          // acceleration force

	float retrothrusterMass;
	float axialRetrothrust;						// breaking force

	float gSuitMass;
	float gFactor;                              // passed on to blackout for g-loc calculation
	float knockoutAcceleration;                 // passed on to blackout for knockouts
	float forceFieldMax;						// used to clamp force field dampening it from causing blackouts

	float impulseConverterMass;
	float clampAngleBoard;                      // max lean angle, determined by impulse converter max Gs

	float forceFieldMass;
	float dragCoefficient;                      // drag coeff. while cruising
	float dragCoefficientBreak;                 // drag coeff. while breaking (sometimes higher)
	float forceFieldDamping;						// damping coeff. while colliding	

	float chassisMass;

	[Tooltip("Attachment point for board model.")]
	public GameObject boardNode;
	GameObject board;

	[Tooltip("Attachment point for force field model.")]
	public GameObject forceFieldNode;
	GameObject forceField;

	[Tooltip("Ratio of lean during banked turns to lean used for G force calculations. 0 to 1 (1 is realistic).")]
	public float leanFactor = 1f;

	[Tooltip("Mass of hoverboarder without upgrades, in kg.")]
	public float baseMass;
	[Tooltip("Spring coefficient for angle correction torque, divided by mass (non-kinematic steering only).")]
	public float torqueSpringM = 37.5f; // 3000 / 80
	float torqueSpring = 3000f;
	[Tooltip("Damping coefficient for angle correction torque, divided by mass (non-kinematic steering only).")]
	public float torqueDampingM = 5f;	// 400 / 80
	float torqueDamping = 400;
	[Tooltip("Corrective drag, cancels out lateral slide after collisions.")]
	public float lateralDragCoefficient = 5f;
	[Tooltip("Speed above which carved turns are applied.")]
	public float carveSpeedMin = 5f;
	[Tooltip("Rotation rate sensitivity when below carveSpeedMin.")]
	public float sensitivityCarveSlow = 2f;
	[Tooltip("Speed below which righting begins after a crash. Kinematic steering only.")]
	public float rightingSpeed = 5f;
	[Tooltip("Angular velocity below which righting begins after a crash. Kinematic steering only.")]
	public float rightingAngularVelocity = 3f;
	[Tooltip("Rate at which roll and pitch are corrected after a crash. Kinematic steering only.")]
	public float rightingRotationRate = 60f;

	[Tooltip("Distance from center node to point of zero hover thrust.")]
	public float hoverThrustDistanceMax = 3f;           // distance down from 0 point to distance of first force
	[Tooltip("Distance from center node to point of 1G hover thrust.")]
	public float hoverThrustDistanceMin = 0.5f;         // distance up from 0 point to distance of max force
	[Tooltip("Distance from center node to point of max hover thrust.")]
	public float hoverThrustDistanceZero = 1.5f;        // distance from COG to ground when stable
	[Tooltip("Maximum hover thrust spring coefficient.")]
	public float hoverThrustGsMax = 3f;                 // thrust at hoverThrustDistanceMin, in Gs
	[Tooltip("Hover thrust damping coefficient.")]
	public float hoverThrustVelocity = 10f;

	[Tooltip("Used to ignore cushion collider during hover thrust raycast.")]
	public LayerMask cushionMask;

	[Tooltip("Minimum speed above which force field collisions apply drag.")]
	public float forceFieldSpeed = 15f;
	[Tooltip("Projection distance of force field.")]
	public float forceFieldReach = 2f;
	[Tooltip("Radius of force field capsule cast.")]
	public float forceFieldRadius = 0.25f;
	[Tooltip("Offset of force field capsule cast sphere centers.")]
	public float forceFieldOffset = 0.9f;

	// reset state when starting a race
	Vector3 startPoint;			
	Quaternion startRotation;

	Rigidbody body;			// player rigidbody


	DriveMode driveMode;	// accel., decel., or neutral

	// player's yaw and roll
	float bodyRotationY = 0f;
	float bodyRotationZ = 0f;

	float bodyAVY = 0f;		// yaw angular velocity

	float carveRotationY;	// turn rate

	float speed = 0f;				// current speed
	Vector3 accelerationVector;		// current acceleration vector
	float acceleration = 0f;		// current acceleration
	float accelerationY = 0f;		// vertical component of acceleration, relative to body orientation
	float thrustMagnitude = 0f;		// current magnitude of thrust (also used for audio driver)
	float hoverMagnitude = 0f;		// current magnitude of hover force (also used for audio driver)
	Vector3 vLast;					// last velocity (used to compute acceleration)

	Quaternion turnRotation;			// yaw quaternion
	Quaternion turnRotationInverse;		// inverse of yaw quaternion

	// true when player is colliding with something
	public bool isColliding
	{
		get { return collisionCount > 0; }
	}

	// Use this for initialization
	void Start()
	{
		body = GetComponent<Rigidbody>();
		startPoint = transform.position;
		startRotation = transform.rotation;

		ResetController();
	}

	// sets total rigidbody mass based on mass of character plus chassis and upgrades
	void UpdateMass()
	{
		if (body == null)
		{
			body = GetComponent<Rigidbody>();		// find rigidbody
		}
		// compute and set mass
		body.mass = baseMass + thrusterMass + retrothrusterMass + gSuitMass + impulseConverterMass + forceFieldMass + chassisMass;
		// compute mass dependent values
		torqueSpring = torqueSpringM * body.mass;
		torqueDamping = torqueDampingM * body.mass;
		forceFieldMax = 0.9f * knockoutAcceleration * body.mass;
	}

	// resets position and velocity to starting values on race reset
	public void ResetController()
	{
		driveMode = DriveMode.Neutral;
		// reset position, rotation, and velocity
		body.velocity = Vector3.zero;
		body.angularVelocity = Vector3.zero;
		vLast = body.velocity;
		body.transform.position = startPoint;
		body.transform.rotation = startRotation;

		// set groundHeightLast
		RaycastHit hitInfo;
		if (Physics.Raycast(body.transform.position, Vector3.down, out hitInfo, 10f, cushionMask.value))
		{
			groundHeightLast = hitInfo.point.y;
		}

		// set rotations
		Vector3 rot = body.transform.rotation.eulerAngles;
		bodyRotationY = rot.y;
		bodyAVY = 0f;

		bodyRotationZ = 0f;

		carveRotationY = 0f;
	}

	// calculate physics-based values, and apply phyics-based forces and torques
	private void FixedUpdate()
	{
		// determine acceleration
		SetAcceleration();

		// calculate target rotations
		CalculateRotations();

		// calculate and appy torque
		Vector3 currentRotation = body.transform.rotation.eulerAngles;
		Vector3 currentVelocity = turnRotationInverse * body.angularVelocity;

		bodyRotationY = PlusMinus180(bodyRotationY, currentRotation.y);
		Vector3 yawTorque = (bodyRotationY - currentRotation.y) * Mathf.Deg2Rad * torqueSpring * Vector3.up;
		yawTorque -= (currentVelocity.y - bodyAVY) * torqueDamping * Vector3.up;

		Vector3 rollTorque = turnRotation * ((bodyRotationZ * leanFactor - PlusMinus180(currentRotation.z, bodyRotationZ * leanFactor)) * Mathf.Deg2Rad * torqueSpring * Vector3.forward);
		rollTorque -= turnRotation * ((currentVelocity.z) * torqueDamping * Vector3.forward);

		Vector3 pitchTorque = turnRotation * (-PlusMinus180(currentRotation.x, 0f) * Mathf.Deg2Rad * torqueSpring * Vector3.right);
		pitchTorque -= turnRotation * (currentVelocity.x * torqueDamping * Vector3.right);

		body.AddTorque(yawTorque + rollTorque + pitchTorque, ForceMode.Force);

		// calculate and apply forces
		Vector3 forceFieldDrag = GetForceFieldDragVector();			// thrust counter to velocity due to forcefield collision
		Vector3 axialThrustVector = GetAxialThrustVector();         // thrust along z axis due to input (accel/decel)
		Vector3 carveThrustVector = CalculateCarve();               // thrust along x axis due to lean (also calculates corresponding angular velocity)
		Vector3 drag = GetDragVector();                             // drag counter to velocity (also calculates speed)
		Vector3 trackingDrag = GetLateralDragVector();              // drag counter to lateral velocity
		Vector3 hoverThrust = GetHoverThrustVector();

		body.AddForce(hoverThrust + drag + trackingDrag + axialThrustVector + carveThrustVector + forceFieldDrag, ForceMode.Force);
	}

	// calculate latest acceleration (absolute and vertical) 
	void SetAcceleration()
	{
		Vector3 a = (body.velocity - vLast) / Time.deltaTime - Physics.gravity;
		vLast = body.velocity;
		acceleration = a.magnitude;
		accelerationVector = Quaternion.Inverse(body.rotation) * a;
		accelerationY = accelerationVector.y;
	}


	// calculate new rotations
	void CalculateRotations()
	{
		bodyAVY = carveRotationY * Mathf.Deg2Rad;
		bodyRotationY += carveRotationY * Time.deltaTime;           // carve rotation set by CalculateCarve()
		turnRotation = Quaternion.Euler(0f, bodyRotationY, 0f);
		turnRotationInverse = Quaternion.Inverse(turnRotation);
	}


	bool forceFieldColliding = false;				// true if the force field is colliding with something, used for audio

	// calculate thrust vector produced by dampening from force field collision
	Vector3 GetForceFieldDragVector()
	{
		if (speed > forceFieldSpeed)	// if player is traveling fast enough for force field to have an effect...
		{
			// bounds of capsule for capsule cast
			Vector3 bottom = transform.position + transform.rotation * Vector3.down * forceFieldOffset;
			Vector3 top = transform.position + transform.rotation * Vector3.up * forceFieldOffset;
			Vector3 velocity = body.velocity;		// used for direction of capsule cast
			if (velocity.y < 0f)
			{
				velocity.y = 0f;        // don't cast downward when descending, to prevent landing deceleration
			}
			// capsule cast
			RaycastHit hit;
			if (Physics.CapsuleCast(bottom, top, forceFieldRadius, velocity, out hit, forceFieldReach, cushionMask.value))
			{
				// if it hits, calculate and apply damping force
				forceFieldColliding = true;
				float forceFactor = 1f;
				if (speed * forceFieldDamping > forceFieldMax)
				{
					forceFactor = forceFieldMax / (speed * forceFieldDamping);		// don't knock them out with the force field
				}
				Vector3 direction = transform.position - hit.point;
				return direction.normalized * speed * forceFieldDamping * forceFactor;
			}
		}

		forceFieldColliding = false;
		return Vector3.zero;
	}

	// calculate thrust vector produced by thrusters and retrothrusters (drive mode set by InputController script)
	Vector3 GetAxialThrustVector()
	{
		switch (driveMode)
		{
			case DriveMode.Accelerate:
				thrustMagnitude = axialThrust;
				return turnRotation * Vector3.forward * axialThrust;
			case DriveMode.Decelerate:
				thrustMagnitude = axialRetrothrust;
				return turnRotation * Vector3.back * axialRetrothrust;
			case DriveMode.Neutral:
				thrustMagnitude = 0f;
				return Vector3.zero;
			default:
				thrustMagnitude = 0f;
				return Vector3.zero;
		}
	}

	// calculate turn force thrust vector (equal and opposite to centripital force), and set carving rotation rate
	Vector3 CalculateCarve()
	{
		Vector3 thrust = Vector3.zero;
		float velocity = (turnRotationInverse * body.velocity).z;	// axial velocity

		if (velocity < carveSpeedMin)
		{	
			// just rotate if traveling slowly (carve physics breaks down)
			carveRotationY = -bodyRotationZ * sensitivityCarveSlow;
		}
		else
		{
			float mass = body.mass;
			float gForce = Physics.gravity.y * mass;
			float cForce = gForce * Mathf.Tan(Mathf.Deg2Rad * bodyRotationZ);	// centripetal force
			thrust = turnRotation * Vector3.right * cForce;
			// f = ma
			// f = m * v^2 / r = m * w^2 * r
			// r = m * v^2 / f
			// w = sqrt (f / (m * r))
			// w = sqrt (f / (m * m * v^2 / f)
			// w = sqrt (f^2 / (m^2 * v^2))
			// w = f / (m * v)
			carveRotationY = Mathf.Rad2Deg * cForce / (mass * velocity);
		}

		return thrust;
	}

	// calculate aerodynamic drag vector (and set speed, to minimize calls to relatively heavy magnitude function)
	Vector3 GetDragVector()
	{
		Vector3 drag = -body.velocity;					// drag is proportional to velocity squared, so start with velocity
		speed = body.velocity.magnitude;                // set speed now to minimize heavy function calls

		if (driveMode == DriveMode.Decelerate)
		{
			drag = drag * speed * dragCoefficientBreak;      // and multiply by magnitude
		}
		else
		{
			drag = drag * speed * dragCoefficient;      // and multiply by magnitude
		}

		return drag;
	}

	// calculate slide-dampening lateral drag vector (returns you to carving after a collision)
	Vector3 GetLateralDragVector()
	{
		float lateralVelocity = (turnRotationInverse * body.velocity).x;
		Vector3 drag = turnRotation * Vector3.left * lateralVelocity * lateralVelocity * Mathf.Sign(lateralVelocity) * lateralDragCoefficient;
		return drag;
	}


	float groundHeightLast;		// y (height) of ground beneath player during last frame

	// calculate thrust vector that keeps you airborne (strength increases with proximity to ground, velocity damped)
	Vector3 GetHoverThrustVector()
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(body.transform.position, Vector3.down, out hitInfo, Mathf.Infinity, cushionMask.value))
		{
			// calculate distance from zero point to ground
			float distance = body.transform.position.y - hoverThrustDistanceZero - hitInfo.point.y;

			// calculate velocity relative to the (changing) ground height
			float vGround = (hitInfo.point.y - groundHeightLast) / Time.deltaTime;
			groundHeightLast = hitInfo.point.y;
			vGround = Mathf.Clamp(vGround, -speed, speed);
			float relativeVelocity = body.velocity.y - vGround;

			// calculate distance based multiplier
			float distanceMult;
			if (distance > hoverThrustDistanceMax)          // ground is too far away
			{
				// distance mult is zero
				return Vector3.zero;
			}
			else if (distance > 0f)                         // ground is beyond zero distance
			{
				// distance mult between 0 and 1
				distanceMult = (hoverThrustDistanceMax - distance) / hoverThrustDistanceMax;
			}
			else
			{
				// distance mult between 1 and hoverThrustGsMax
				distanceMult = (1f + (1 - hoverThrustGsMax) * distance / hoverThrustDistanceMin);
			}

			// calculate distance based force (spring)
			float distanceForce = Mathf.Abs(Physics.gravity.y) * body.mass * distanceMult;

			// calculate velocity based forced (damping)
			float velocityForce = distanceMult * relativeVelocity * body.mass * hoverThrustVelocity;

			Vector3 thrust;
			thrust = Vector3.up * (distanceForce - velocityForce);
			hoverMagnitude = thrust.y;
			return thrust;
		}
		else
		{
			groundHeightLast = body.transform.position.y - hoverThrustDistanceZero;
			hoverMagnitude = 0f;
			return Vector3.zero;
		}
	}

	// public getter for speed
	public float GetSpeed()
	{
		return speed;
	}

	// public getter for acceleration magnitude
	public float GetAcceleration()
	{
		return acceleration;
	}
	 
	// public getter for vertical acceleration (used in G-LOC calculations)
	public float GetAccelerationY()
	{
		return accelerationY;
	}

	// public getter for acceleration vector
	public Vector3 GetAccelerationVector()
	{
		return accelerationVector;
	}

	// conditions euler angles (corrects "angle" to be within 180 degrees of "target")
	float PlusMinus180(float angle, float target)
	{
		while (angle > target + 180f)
		{
			angle -= 360f;
		}
		while (angle <= target - 180f)
		{
			angle += 360f;
		}
		return angle;
	}

	// applies thruster upgrade
	public void InstallThruster(Thruster thruster)
	{
		axialThrust = thruster.thrust;
		thrusterMass = thruster.mass;
		UpdateMass();
	}

	// applies retrothruster upgrade
	public void InstallRetrothruster(Retrothruster retrothruster)
	{
		axialRetrothrust = retrothruster.thrust;
		retrothrusterMass = retrothruster.mass;
		UpdateMass();
	}

	// applies g-suit upgrade
	public void InstallGSuit(GSuit gSuit)
	{
		gFactor = gSuit.gFactor;
		knockoutAcceleration = gSuit.knockoutAcceleration;
		// f = ma
		gSuitMass = gSuit.mass;
		UpdateMass();
	}

	// applies impulse converter upgrade
	public void InstallImpulseConverter(ImpulseConverter impulseConverter)
	{
		// tan bank angle = gMax / g
		// bank angle = atan (gMax / g)
		clampAngleBoard = Mathf.Atan(impulseConverter.gMax) * Mathf.Rad2Deg;
		impulseConverterMass = impulseConverter.mass;
		UpdateMass();
	}

	// applies force field upgrade
	public void InstallForceField(ForceField forceField)
	{
		// spawn force field model
		if (this.forceField != null)
		{
			Destroy(this.forceField);
		}
		if (forceField.model != null)
		{
			this.forceField = GameObject.Instantiate(forceField.model, forceFieldNode.transform);
		}
		else
		{
			this.forceField = null;
		}

		dragCoefficient = forceField.dragCoefficient;
		dragCoefficientBreak = forceField.dragCoefficientBreak;
		forceFieldDamping = forceField.damping;
		forceFieldMass = forceField.mass;
		UpdateMass();
	}

	// applies chassis upgrade
	public void InstallChassis(Chassis chassis)
	{
		// spawn chassis
		if (board != null)
		{
			Destroy(board);
		}
		if (chassis.model != null)
		{
			board = GameObject.Instantiate(chassis.model, boardNode.transform);
		}
		else
		{
			board = null;
		}

		chassisMass = chassis.mass;
		UpdateMass();
	}

	// getter for values driving audio modulation
	public float GetSoundDriver(SoundDriverCode code)
	{
		// supplies variables that drive sound effect pitch and volume
		switch (code)
		{
			case SoundDriverCode.ForceField:
				if (forceFieldColliding)
				{
					return speed;
				}
				else
				{
					return 0f;
				}
			case SoundDriverCode.Hover:
				return hoverMagnitude;
			case SoundDriverCode.Speed:
				return speed;
			case SoundDriverCode.Thrust:
				return thrustMagnitude;
			default:
				return 0f;
		}
	}

	// public getter for turn rotation
	public Quaternion GetTurnRotation()
	{
		return turnRotation;
	}

	int collisionCount = 0;

	// collisioncount > 0 means forcefield is colliding with an object
	// increment collision count when collision starts
	private void OnCollisionEnter(Collision collision)
	{
		collisionCount += 1;
	}

	// decrement collision count when collision stops
	private void OnCollisionExit(Collision collision)
	{
		collisionCount -= 1;
	}

	// public setter function for acceleration (accel., decel., neutral)
	public void SetDriveMode(DriveMode driveMode)
	{
		this.driveMode = driveMode;
	}

	// public setter function for banking angle
	public void SetZRotation(float rotation)
	{
		bodyRotationZ = Mathf.Clamp(rotation, -clampAngleBoard, clampAngleBoard);
	}

	// public getter function for banking angle
	public float GetZRotation()
	{
		return bodyRotationZ;
	}

	// public getter function for maximum bank angle
	public float GetClampAngleBoard()
	{
		return clampAngleBoard;
	}

	// public getter function for gFactor (used in G-LOC calculations)
	public float GetGFactor()
	{
		return gFactor;
	}

	// public getter function for knockout acceleration (used in crash blackout calculations)
	public float GetKnockoutAcceleration()
	{
		return knockoutAcceleration;
	}
}
