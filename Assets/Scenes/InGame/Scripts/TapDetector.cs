using UnityEngine;
using System.Collections;
using Leap;

public class TapDetector : MonoBehaviour
{
	public bool Right;
	public bool Left;
	public bool DownLeft;
	public bool DownRight;
	public GameObject RightLine;
	public GameObject LeftLine;
	public GameObject DownRightLine;
	public GameObject DownLeftLine;
	Controller leap;
	Color grey;
	bool CR;
	bool CL;
	bool CDL;
	bool CDR;

	// Use this for initialization
	void Start ()
	{
		Right = Left = DownLeft = DownRight = false;
		CR = CL = CDL = CDR = false;
		leap = new Controller ();
		grey = new Color (0.5f, 0.5f, 0.5f);
	}



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
		Frame frame = leap.Frame ();
		foreach (Hand hand in frame.Hands) {
			// Debug.Log (hand.Fingers.Frontmost.TipPosition);
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
			}
		}

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
