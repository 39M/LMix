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
	AudioSource hitSE;
	AudioSource missSE;
	GamePlayer status;
	Vector FingerPos;
	Vector3 notePos;
	float lim_x_low, lim_x_high, lim_y_low, lim_y_high, lim_z_low, lim_z_high;
	bool hit;

	void Start ()
	{
		leap = new Controller ();
		hit = pause = stop = false;
		hitSE = GetComponent<AudioSource> ();
		missSE = hitSE;
		status = GameObject.Find ("GamePlayer").GetComponent ("GamePlayer") as GamePlayer;
	}

	void Update ()
	{
		if (status.stop)
			Destroy (gameObject);
		if (status.pause)
			return;

		if ((notePos.z <= -2 && !missSE.isPlaying) || (hit && !hitSE.isPlaying)) {
			Destroy (gameObject);
		}

		transform.Translate (new Vector3 (10, 0, 0) * Time.deltaTime * speed);
		notePos = transform.position;
		if (!hit && notePos.z < 2 && notePos.z > -2) {
			lim_z_low = -175;
			lim_z_high = 175;
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
							rt.eulerAngles = new Vector3 (0, -90, 0);
						else if (transform.rotation.eulerAngles.x == 90)
							rt.eulerAngles = new Vector3 (0, 0, 0);
						else// if (transform.rotation.eulerAngles.x == 180)
							rt.eulerAngles = new Vector3 (0, 90, 0);
						//Debug.Log(transform.rotation.eulerAngles.x / 2);
						GameObject tmp = (GameObject)Instantiate (score, transform.position, rt);
						if (Mathf.Abs (notePos.z) < 0.75) {
							tmp.GetComponent<TextMesh> ().text = "Perfect!";
							status.ComboCounter++;
							status.ScoreCounter += 300 * status.ComboCounter;
						} else if (Mathf.Abs (notePos.z) < 1.5) {
							tmp.GetComponent<TextMesh> ().text = "Good!";
							status.ComboCounter++;
							status.ScoreCounter += 100 * status.ComboCounter;
						} else if (Mathf.Abs (notePos.z) < 1.75) {
							tmp.GetComponent<TextMesh> ().text = "Bad!";
							status.ComboCounter = 0;
							status.ScoreCounter += 50 * status.ComboCounter;
						} else {
							tmp.GetComponent<TextMesh> ().text = "Miss!";
							status.ComboCounter = 0;
						}
						status.ScoreText.text = "Score: " + status.ScoreCounter.ToString ();
						status.ComboText.text = "Combo: " + status.ComboCounter.ToString ();

						hit = true;
						hitSE.Play ();
						GetComponent<Renderer> ().enabled = false;
						//Destroy (gameObject);
						return;
					}
				}
			}
		}

		if (notePos.z <= -2 && !hit && !missSE.isPlaying) {
			Quaternion rt = Quaternion.identity;
			if (transform.rotation.eulerAngles.x == 0)
				rt.eulerAngles = new Vector3 (0, -90, 0);
			else if (transform.rotation.eulerAngles.x == 90)
				rt.eulerAngles = new Vector3 (0, 0, 0);
			else// if (transform.rotation.eulerAngles.x == 180)
				rt.eulerAngles = new Vector3 (0, 90, 0);

			GameObject tmp = (GameObject)Instantiate (score, transform.position, rt);
			tmp.GetComponent<TextMesh> ().text = "Miss!";
			status.ComboCounter = 0;
			GetComponent<Renderer> ().enabled = false;
			missSE.Play ();
		}
	}
}

// H: 73  1.25    96       -83         2      1.75
// leap vector / 50 = unity vector3