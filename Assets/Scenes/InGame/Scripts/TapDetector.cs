using UnityEngine;
using System.Collections;
using Leap;

public class TapDetector : MonoBehaviour
{
	public bool Right;
	public bool Left;
	public bool DownLeft;
	public bool DownRight;
	public bool Pause;
	public bool Exit;
	public bool PauseTrigger;
	public GameObject RightLine;
	public GameObject LeftLine;
	public GameObject DownRightLine;
	public GameObject DownLeftLine;
	Color grey;
	bool CR;
	bool CL;
	bool CDL;
	bool CDR;
	bool CPause;
	KinectManager manager;
	float counter = 1f;

	// Use this for initialization
	void Start ()
	{
		Right = Left = DownLeft = DownRight = false;
		Pause = Exit = CPause = false;
		CR = CL = CDL = CDR = false;
		grey = new Color (0.5f, 0.5f, 0.5f);
		manager = GameObject.Find("KinectObject").GetComponent("KinectManager") as KinectManager;
	}

	// x 100  y 400

	// Update is called once per frame
	void Update ()
	{


		/*
		int width = 200, y_base = 0;
		float y_basef = 1.15f;

		if (notePos.x > 0) {
			if (notePos.y < y_basef - 0.5) {
				lim_x_low = 0;
				lim_x_high = 200;
				lim_y_low = y_base - 80;
				lim_y_high = lim_y_low + width;	// 80
				track = 0;
			} else if (notePos.y < y_basef + 2) {
				lim_x_low = 170;
				lim_x_high = 300;
				lim_y_low = y_base + 50;
				lim_y_high = lim_y_low + width;
				track = 2;
			}
		} else {
			if (notePos.y < y_basef - 0.5) {
				lim_x_low = -200;
				lim_x_high = 0;
				lim_y_low = y_base - 80;
				lim_y_high = lim_y_low + width;	// 80
				track = 1;
			} else if (notePos.y < y_basef + 2) {
				lim_x_low = -300;
				lim_x_high = -170;
				lim_y_low = y_base + 50;
				lim_y_high = lim_y_low + width;
				track = 3;
			}
		}
*/		

		CR = Right;
		CL = Left;
		CDL = DownLeft;
		CDR = DownRight;
		Right = Left = DownLeft = DownRight = false;
		Pause = false;

		//---------kincet--------------//
		if (manager.IsUserDetected()) {
			uint UserID = manager.GetPlayer1ID();
			Vector3 leftHandPos = manager.GetJointPosition(UserID, 7);
			Vector3 rightHandPos = manager.GetJointPosition(UserID, 11);

			if (counter < 0) {
				Debug.Log("-------------------");
				Debug.Log(leftHandPos);
				Debug.Log(rightHandPos);
				counter = 1f;
			} else {
				counter -= Time.deltaTime;
			}

			if (rightHandPos.x > 0f && rightHandPos.x < 0.3f && rightHandPos.y < 1f) {
				DownRight = true;
			}
			if (leftHandPos.x < 0f && leftHandPos.x > -0.3f && leftHandPos.y < 1f) {
				DownLeft = true;
			}
			if (leftHandPos.x < -0.2 && leftHandPos.y > 0.8f && leftHandPos.y < 2f) {
				Left = true;
			}
			if (rightHandPos.x > 0.2 && rightHandPos.y > 0.8f && rightHandPos.y < 2f) {
				Right = true;
			} 
			if (leftHandPos.y > 2f) 
				if (leftHandPos.x > 0.3f)
					Exit = true;
			if (rightHandPos.y > 2f)
				if (rightHandPos.x < -0.3f)
					Pause = true;
		}


		//---------kincet--------------//


		/*Frame frame = leap.Frame ();

		foreach (Hand hand in frame.Hands) {
			//Debug.Log (hand.Fingers.Frontmost.TipPosition);
			foreach (Finger finger in hand.Fingers) {
				Vector FingerPos = finger.TipPosition;
				// Destroy note if tap success
				if (FingerPos.x > 0 && FingerPos.x < 200 && FingerPos.y < 120) {
					DownRight = true;
				}
				if (FingerPos.x < 0 && FingerPos.x > -200 && FingerPos.y < 120) {
					DownLeft = true;
				}
				if (FingerPos.x < -170 && FingerPos.y > 50 && FingerPos.y < 250) {
					Left = true;
				}
				if (FingerPos.x > 170 && FingerPos.y > 50 && FingerPos.y < 250) {
					Right = true;
				} 
				if (FingerPos.y > 400) {
					if (FingerPos.x > 100)
						Exit = true;
					if (FingerPos.x < -100)
						Pause = true;
				}
			}
		}*/

		if (Input.GetKey (KeyCode.D)) {
			Left = true;
		}
		if (Input.GetKey (KeyCode.F)) {
			DownLeft = true;
		}
		if (Input.GetKey (KeyCode.J)) {
			DownRight = true;
		}
		if (Input.GetKey (KeyCode.K)) {
			Right = true;
		}
		if (Input.GetKey (KeyCode.Escape)) {
			Exit = true;
		}
		if (Input.GetKey (KeyCode.Space)) {
			Pause = true;
		}


		if (CPause != Pause && Pause) {
			PauseTrigger = true;
			Debug.Log ("Trigger");
			Debug.Log (Pause);
		}

		CPause = Pause;

		if (DownRight != CDR)
		if (DownRight) {
			DownRightLine.GetComponent <MeshRenderer> ().material.color = Color.white;
		} else {
			DownRightLine.GetComponent <MeshRenderer> ().material.color = grey;
		}

		if (DownLeft != CDL)
		if (DownLeft) {
			DownLeftLine.GetComponent <MeshRenderer> ().material.color = Color.white;
		} else {
			DownLeftLine.GetComponent <MeshRenderer> ().material.color = grey;
		}

		if (Right != CR)
		if (Right) {
			RightLine.GetComponent <MeshRenderer> ().material.color = Color.white;
		} else {
			RightLine.GetComponent <MeshRenderer> ().material.color = grey;
		}

		if (Left != CL)
		if (Left) {
			LeftLine.GetComponent <MeshRenderer> ().material.color = Color.white;
		} else {
			LeftLine.GetComponent <MeshRenderer> ().material.color = grey;
		}
	}
}
