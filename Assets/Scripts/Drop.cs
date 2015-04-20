using UnityEngine;
using System.Collections;
using Leap;

public class Drop : MonoBehaviour
{
	public float speed;
	Controller leap;
	Vector FingerPos;
	float lim_x_low, lim_x_high, lim_y_low, lim_y_high, lim_z_low, lim_z_high;
	Vector3 notePos;

	// Use this for initialization
	void Start ()
	{
		leap = new Controller ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.Translate (new Vector3 (15, 0, 0) * Time.deltaTime * speed);
		notePos = transform.position;
		if (notePos.z < 1.75 && notePos.z > -1.75) {
			lim_z_low = -175;
			lim_z_high = 0;
			if (notePos.x > 0) {
				if (notePos.y < -0.5) {
					lim_x_low = 0;
					lim_x_high = 200;
					lim_y_low = -100;
					lim_y_high = 80;

				} else if (notePos.y < 2) {
					lim_x_low = 170;
					lim_x_high = 300;
					lim_y_low = 50;
					lim_y_high = 250;
				} else {
					lim_x_low = 170;
					lim_x_high = 300;
					lim_y_low = 250;
					lim_y_high = 450;
				}
			} else {
				if (notePos.y < -0.5) {
					lim_x_low = -200;
					lim_x_high = 0;
					lim_y_low = -100;
					lim_y_high = 80;
				} else if (notePos.y < 2) {
					lim_x_low = -300;
					lim_x_high = -170;
					lim_y_low = 50;
					lim_y_high = 250;
				} else {
					lim_x_low = -300;
					lim_x_high = -170;
					lim_y_low = 250;
					lim_y_high = 450;
				}
			}

			Frame frame = leap.Frame ();
			foreach (Hand hand in frame.Hands) {
				Debug.Log (hand.Fingers.Frontmost.TipPosition);
				foreach (Finger finger in hand.Fingers) {
					FingerPos = finger.TipPosition;
					// Destroy note if tap success
					if (lim_y_low < FingerPos.y && FingerPos.y < lim_y_high && 
						lim_x_low < FingerPos.x && FingerPos.x < lim_x_high && 
						lim_z_low < FingerPos.z && FingerPos.z < lim_z_high) {
						Destroy (gameObject);
						return;
					}
				}
			}
		}

		if (notePos.z < -10)
			Destroy (gameObject);
	}
}

// H: 73  1.25    96       -83         2      1.75
// leap vector / 50 = unity vector3