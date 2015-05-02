using UnityEngine;
using System.Collections;
using Leap;

public class Drop : MonoBehaviour
{
	public float speed;
	public GameObject score;
	public bool pause;
	public bool stop;
	Controller leap;
	AudioSource audio;
	GamePlayer status;
	Vector FingerPos;
	float lim_x_low, lim_x_high, lim_y_low, lim_y_high, lim_z_low, lim_z_high;
	Vector3 notePos;
	bool hit;

	// Use this for initialization
	void Start ()
	{
		leap = new Controller ();
		hit = pause = stop = false;
		audio = GetComponent<AudioSource>();
		status = GameObject.Find ("GamePlayer").GetComponent ("GamePlayer") as GamePlayer;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (status.stop)
			Destroy (gameObject);
		if (status.pause)
			return;

		transform.Translate (new Vector3 (10, 0, 0) * Time.deltaTime * speed);
		notePos = transform.position;
		if (!hit && notePos.z < 1.75 && notePos.z > -1.75) {
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
				// Debug.Log (hand.Fingers.Frontmost.TipPosition);
				foreach (Finger finger in hand.Fingers) {
					FingerPos = finger.TipPosition;
					// Destroy note if tap success
					if (lim_y_low < FingerPos.y && FingerPos.y < lim_y_high && 
						lim_x_low < FingerPos.x && FingerPos.x < lim_x_high && 
						lim_z_low < FingerPos.z && FingerPos.z < lim_z_high) {

						Quaternion rt = Quaternion.identity;
						if (transform.rotation.eulerAngles.x == 0)
							rt.eulerAngles = new Vector3(0, -90, 0);
						else if (transform.rotation.eulerAngles.x == 90)
							rt.eulerAngles = new Vector3(0, 0, 0);
						else// if (transform.rotation.eulerAngles.x == 180)
							rt.eulerAngles = new Vector3(0, 90, 0);
						//Debug.Log(transform.rotation.eulerAngles.x / 2);
						GameObject tmp = (GameObject) Instantiate (score, transform.position, rt);
						if (Mathf.Abs(notePos.z) < 0.5)
							tmp.GetComponent<TextMesh>().text = "Perfect!";
						else if (Mathf.Abs(notePos.z) < 1)
							tmp.GetComponent<TextMesh>().text = "Good!";
						else if (Mathf.Abs(notePos.z) < 1.5)
							tmp.GetComponent<TextMesh>().text = "Bad!";
						else
							tmp.GetComponent<TextMesh>().text = "Miss!";

						hit = true;
						audio.Play();
						GetComponent<Renderer>().enabled = false;
						//Destroy (gameObject);
						return;
					}
				}
			}
		}

		if (notePos.z < -10 || (hit && !audio.isPlaying))
			Destroy (gameObject);
	}
}

// H: 73  1.25    96       -83         2      1.75
// leap vector / 50 = unity vector3