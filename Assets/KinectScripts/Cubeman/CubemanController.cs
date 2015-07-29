using UnityEngine;
using System;
using System.Collections;

public class CubemanController : MonoBehaviour 
{
	public bool MoveVertically = false;
	public bool MirroredMovement = false;

	//public GameObject debugText;
	
	public GameObject Hip_Center;
	public GameObject Spine;
	public GameObject Shoulder_Center;
	public GameObject Head;
	public GameObject Shoulder_Left;
	public GameObject Elbow_Left;
	public GameObject Wrist_Left;
	public GameObject Hand_Left;
	public GameObject Shoulder_Right;
	public GameObject Elbow_Right;
	public GameObject Wrist_Right;
	public GameObject Hand_Right;
	public GameObject Hip_Left;
	public GameObject Knee_Left;
	public GameObject Ankle_Left;
	public GameObject Foot_Left;
	public GameObject Hip_Right;
	public GameObject Knee_Right;
	public GameObject Ankle_Right;
	public GameObject Foot_Right;

	public LineRenderer SkeletonLine;
	
	private GameObject[] bones; 
	private LineRenderer[] lines;
	private int[] parIdxs;
	
	private Vector3 initialPosition;
	private Quaternion initialRotation;
	private Vector3 initialPosOffset = Vector3.zero;
	private uint initialPosUserID = 0;
	
	
	void Start () 
	{
		//store bones in a list for easier access
		bones = new GameObject[] {
			Hip_Center, Spine, Shoulder_Center, Head,  // 0 - 3
			Shoulder_Left, Elbow_Left, Wrist_Left, Hand_Left,  // 4 - 7
			Shoulder_Right, Elbow_Right, Wrist_Right, Hand_Right,  // 8 - 11
			Hip_Left, Knee_Left, Ankle_Left, Foot_Left,  // 12 - 15
			Hip_Right, Knee_Right, Ankle_Right, Foot_Right  // 16 - 19
		};

		parIdxs = new int[] {
			0, 0, 1, 2,
			2, 4, 5, 6,
			2, 8, 9, 10,
			0, 12, 13, 14,
			0, 16, 17, 18
		};
		
		// array holding the skeleton lines
		lines = new LineRenderer[bones.Length];
		
		if(SkeletonLine)
		{
			for(int i = 0; i < lines.Length; i++)
			{
				lines[i] = Instantiate(SkeletonLine) as LineRenderer;
				lines[i].transform.parent = transform;
			}
		}
		
		initialPosition = transform.position;
		initialRotation = transform.rotation;
		//transform.rotation = Quaternion.identity;
	}
	
	// Update is called once per frame
	void Update () 
	{
		KinectManager manager = KinectManager.Instance;

		// get 1st player
		uint playerID = manager != null ? manager.GetPlayer1ID() : 0;
		
		if(playerID <= 0)
		{
			// reset the pointman position and rotation
			if(transform.position != initialPosition)
			{
				transform.position = initialPosition;
			}
			
			if(transform.rotation != initialRotation)
			{
				transform.rotation = initialRotation;
			}
			
			for(int i = 0; i < bones.Length; i++) 
			{
				bones[i].gameObject.SetActive(true);
				
				bones[i].transform.localPosition = Vector3.zero;
				bones[i].transform.localRotation = Quaternion.identity;
				
				if(SkeletonLine)
				{
					lines[i].gameObject.SetActive(false);
				}
			}
			
			return;
		}

		// set the user position in space
		Vector3 posPointMan = manager.GetUserPosition(playerID);
		posPointMan.z = !MirroredMovement ? -posPointMan.z : posPointMan.z;
		
		// store the initial position
		if(initialPosUserID != playerID)
		{
			initialPosUserID = playerID;
			initialPosOffset = transform.position - (MoveVertically ? posPointMan : new Vector3(posPointMan.x, 0, posPointMan.z));
		}
		
		transform.position = initialPosOffset + (MoveVertically ? posPointMan : new Vector3(posPointMan.x, 0, posPointMan.z));

		// update the local positions of the bones
		for(int i = 0; i < bones.Length; i++) 
		{
			if(bones[i] != null)
			{
				int joint = MirroredMovement ? KinectWrapper.GetSkeletonMirroredJoint(i): i;
				
				if(manager.IsJointTracked(playerID, joint))
				{
					bones[i].gameObject.SetActive(true);
					
					Vector3 posJoint = manager.GetJointPosition(playerID, joint);
					posJoint.z = !MirroredMovement ? -posJoint.z : posJoint.z;

					Quaternion rotJoint = manager.GetJointOrientation(playerID, joint, !MirroredMovement);
					rotJoint = initialRotation * rotJoint;

					posJoint -= posPointMan;

					if(MirroredMovement)
					{
						posJoint.x = -posJoint.x;
						posJoint.z = -posJoint.z;
					}

					bones[i].transform.localPosition = posJoint;
					bones[i].transform.rotation = rotJoint;
				}
				else
				{
					bones[i].gameObject.SetActive(false);
				}
			}	
		}

		if(SkeletonLine)
		{
			for(int i = 0; i < bones.Length; i++) 
			{
				bool bLineDrawn = false;

				if(bones[i] != null)
				{
					if(bones[i].gameObject.activeSelf)
					{
						Vector3 posJoint = bones[i].transform.position;

						int parI = parIdxs[i];
						Vector3 posParent = bones[parI].transform.position;

						if(bones[parI].gameObject.activeSelf)
						{
							lines[i].gameObject.SetActive(true);
							
							//lines[i].SetVertexCount(2);
							lines[i].SetPosition(0, posParent);
							lines[i].SetPosition(1, posJoint);

							bLineDrawn = true;
						}
					}
				}	

				if(!bLineDrawn)
				{
					lines[i].gameObject.SetActive(false);
				}
			}
		}

	}

}
