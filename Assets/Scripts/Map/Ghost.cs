using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// struct used to represent a frame
struct GhostFrame
{
	public float frameTime;
	public int checkpointCount;
	public Vector3 position;
	public Quaternion rotation;
}

// class used to play back a ghost recording associated with the course it is attached to
// to-do: use spline interpolation to smooth out acceleration and path (big mathematical headache...)
public class Ghost : MonoBehaviour {
	public TextAsset ghostFile;			// file where ghost recording data is stored
	public GameObject ghostModel;		// prefab for ghost model

	GameObject ghost;					// ghost prefab instance

	GhostFrame[] ghostFrames;			// ghost keyframes used for playback, populated from ghostFile

	int frame;							// current frame in playback
	int nextFrame;						// next frame in playback
	float frameTime;					// current time in playback
	bool ghosting = false;				// is the ghost currently being played?

	// Use this for initialization
	void Start ()
	{
		if (ghostFile != null)		// if there is a ghost file...
		{
			// populate ghostFrames array from its data
			string[] newline = { System.Environment.NewLine };		// newline delimiter
			string[] tab = new string[1];							// tab delimiter
			tab[0] = "\t";
			// split the text from the ghostFile into lines around the newline delimiter
			string[] ghostData = ghostFile.text.Split(newline, System.StringSplitOptions.RemoveEmptyEntries);
			// initialize the ghostFrames array based on the number of lines found
			ghostFrames = new GhostFrame[ghostData.Length];
			// for each line of data...
			for (int i = 0, j = 0; i < ghostData.Length; i++)
			{
				// split the line of data into elements around the tab delimiter
				string[] line = ghostData[i].Split(tab, System.StringSplitOptions.None);
				if (line.Length >= 9)		// if it's a valid line...
				{
					ghostFrames[j].frameTime = float.Parse(line[0]);		// frame time
					ghostFrames[j].checkpointCount = int.Parse(line[1]);	// checkpoint count
					ghostFrames[j].position.x = float.Parse(line[2]);		// x position
					ghostFrames[j].position.y = float.Parse(line[3]);		// y position
					ghostFrames[j].position.z = float.Parse(line[4]);		// z position
					ghostFrames[j].rotation.w = float.Parse(line[5]);		// w rotation component
					ghostFrames[j].rotation.x = float.Parse(line[6]);		// x rotation component
					ghostFrames[j].rotation.y = float.Parse(line[7]);		// y rotation component
					ghostFrames[j].rotation.z = float.Parse(line[8]);		// z rotation component
					j++;													// increment frame index
				}
			}
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (ghosting)		// if the ghost is being played back...
		{
			frameTime += Time.deltaTime;							// increment frame time
			while (frameTime >= ghostFrames[frame + 1].frameTime)	// advance the current frame as needed to match current frame time
			{
				frame += 1;
				nextFrame = frame + 1;
				if (nextFrame == ghostFrames.Length)		// wrap around if the end of the array has been reached
				{
					frameTime = frameTime - ghostFrames[frame].frameTime + ghostFrames[0].frameTime;
					frame = 0;
					nextFrame = 1;
				}
			}
			// calculate t, or fraction of progress between current frame and next frame
			float t = (frameTime - ghostFrames[frame].frameTime) / (ghostFrames[nextFrame].frameTime - ghostFrames[frame].frameTime);
			// use linear interpolation to find position and rotation, given t calculated above
			ghost.transform.position = Vector3.Lerp(ghostFrames[frame].position, ghostFrames[frame + 1].position, t);
			ghost.transform.rotation = Quaternion.Lerp(ghostFrames[frame].rotation, ghostFrames[frame + 1].rotation, t);
		}
	}

	// starts a ghost playback from the supplied checkpoint index
	public void StartGhost(int checkpointIndex)
	{
		if (ghostFile != null)	// if a ghost file is present
		{
			if (ghost == null)
			{
				ghost = GameObject.Instantiate(ghostModel);		// instantiate the ghost prefab if it hasn't already
			}
			else
			{
				ghost.SetActive(true);							// enable it if it has
			}
			frame = 0;											// set frame and nextFrame to 0 and 1
			nextFrame = 1;
			for (int i = 0, j = 0; i < ghostFrames.Length - 1 && j < checkpointIndex; i++)	// advance frame until checkpoint is reached
			{
				frame = i;
				nextFrame = frame + 1;
				j += ghostFrames[frame].checkpointCount - ghostFrames[nextFrame].checkpointCount;	// accounts for multiple checkpoints per frame
			}
			// set initial frameTime, position, and rotation
			frameTime = ghostFrames[frame].frameTime;
			ghost.transform.position = ghostFrames[frame].position;
			ghost.transform.rotation = ghostFrames[frame].rotation;
			// indicate that ghosting has begun
			ghosting = true;
		}
	}

	// stops a ghost playback
	public void StopGhost()
	{
		if (ghosting)
		{
			ghosting = false;			// set ghosting to false
			ghost.SetActive(false);		// hide the ghost instance
		}
	}
}
