using UnityEngine;
using System.Collections;
using System.IO;

public class GetJointPositionDemo : MonoBehaviour 
{
	// the joint we want to track
	public KinectWrapper.NuiSkeletonPositionIndex joint = KinectWrapper.NuiSkeletonPositionIndex.HandRight;

	// joint position at the moment, in Kinect coordinates
	public Vector3 outputPosition;

	// if it is saving data to a csv file or not
	public bool isSaving = false;

	// how many seconds to save data into the csv file, or 0 to save non-stop
	public float secondsToSave = 0f;

	// path to the csv file (;-limited)
	public string saveFilePath = "joint_pos.csv";


	// start time of data saving to csv file
	private float saveStartTime = -1f;


	void Update () 
	{
		if(isSaving)
		{
			// create the file, if needed
			if(!File.Exists(saveFilePath))
			{
				using(StreamWriter writer = File.CreateText(saveFilePath))
				{
					// csv file header
					string sLine = "time;joint;pos_x;pos_y;poz_z";
					writer.WriteLine(sLine);
				}
			}

			// check the start time
			if(saveStartTime < 0f)
			{
				saveStartTime = Time.time;
			}
		}

		// get the joint position
		KinectManager manager = KinectManager.Instance;

		if(manager && manager.IsInitialized())
		{
			if(manager.IsUserDetected())
			{
				uint userId = manager.GetPlayer1ID();

				if(manager.IsJointTracked(userId, (int)joint))
				{
					// output the joint position for easy tracking
					Vector3 jointPos = manager.GetJointPosition(userId, (int)joint);
					outputPosition = jointPos;

					if(isSaving)
					{
						if((secondsToSave == 0f) || ((Time.time - saveStartTime) <= secondsToSave))
						{
							using(StreamWriter writer = File.AppendText(saveFilePath))
							{
								string sLine = string.Format("{0:F3};{1};{2:F3};{3:F3};{4:F3}", Time.time, (int)joint, jointPos.x, jointPos.y, jointPos.z);
								writer.WriteLine(sLine);
							}
						}
					}
				}
			}
		}

	}

}
