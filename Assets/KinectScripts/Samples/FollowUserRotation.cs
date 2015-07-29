using UnityEngine;
using System.Collections;

public class FollowUserRotation : MonoBehaviour 
{
	void Update () 
	{
		KinectManager manager = KinectManager.Instance;

		if(manager && manager.IsInitialized())
		{
			if(manager.IsUserDetected())
			{
				uint userId = manager.GetPlayer1ID();

				if(manager.IsJointTracked(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft) &&
				   manager.IsJointTracked(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight))
				{
					Vector3 posLeftShoulder = manager.GetJointPosition(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderLeft);
					Vector3 posRightShoulder = manager.GetJointPosition(userId, (int)KinectWrapper.NuiSkeletonPositionIndex.ShoulderRight);

					posLeftShoulder.z = -posLeftShoulder.z;
					posRightShoulder.z = -posRightShoulder.z;

					Vector3 dirLeftRight = posRightShoulder - posLeftShoulder;
					dirLeftRight -= Vector3.Project(dirLeftRight, Vector3.up);

					Quaternion rotationShoulders = Quaternion.FromToRotation(Vector3.right, dirLeftRight);

					transform.rotation = rotationShoulders;
				}
			}
		}
	}
}
