using UnityEngine;
using System.Collections;
using Leap;

public class SlideDrop : MonoBehaviour
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
	bool miss;

	void Start ()
	{
		leap = new Controller ();
		hit = miss = pause = stop = false;

		status = GameObject.Find ("GamePlayer").GetComponent ("GamePlayer") as GamePlayer;
		if (status.enableSE) {
			hitSE = GetComponent<AudioSource> ();
			hitSE.clip = Resources.Load ("Music/" + status.beatmapName + "/hit") as AudioClip;
			missSE = hitSE;
		}
	}

	void Update ()
	{
		if (status.stop)
			Destroy (gameObject);
		if (status.pause)
			return;

		if ((notePos.z <= -1.75 && !missSE.isPlaying) || (hit && !hitSE.isPlaying)) {
			Destroy (gameObject);
		}

		if (!hit)
			transform.Translate (new Vector3 (10, 0, 0) * Time.deltaTime * speed);
		notePos = transform.position;
		if (!hit && notePos.z <= 0 && notePos.z > -1.75) {
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
						TextMesh tmp = ((GameObject)Instantiate (score, transform.position, rt)).GetComponent<TextMesh> ();
						if (notePos.z > -0.75) {
							tmp.text = "Perfect!";
							tmp.color = Color.yellow;
							status.ComboCounter++;
							status.PerfectCount++;
							status.ScoreCounter += 300 * status.ComboCounter;
						} else if (notePos.z > -1.5) {
							tmp.text = "Good!";
							tmp.color = Color.green;
							status.ComboCounter++;
							status.GoodCount++;
							status.ScoreCounter += 100 * status.ComboCounter;
						} else if (notePos.z > -1.75) {
							tmp.text = "Bad!";
							tmp.color = Color.blue;
							status.ComboCounter = 0;
							status.BadCount++;
							status.ScoreCounter += 50 * status.ComboCounter;
						} else {
							tmp.text = "Miss!";
							tmp.color = Color.red;
							status.MissCount++;
							status.ComboCounter = 0;
						}
						status.ScoreText.text = "Score: " + status.ScoreCounter.ToString ();
						status.ComboText.text = "Combo: " + status.ComboCounter.ToString ();

						hit = true;
						if (status.enableSE) {
							hitSE.enabled = true;
							hitSE.Play ();
						}
						GetComponent<Renderer> ().enabled = false;
						//Destroy (gameObject);
						return;
					}
				}
			}
		}

		if (notePos.z <= -1.75 && !hit && !missSE.isPlaying) {
			Quaternion rt = Quaternion.identity;
			if (transform.rotation.eulerAngles.x == 0)
				rt.eulerAngles = new Vector3 (0, -90, 0);
			else if (transform.rotation.eulerAngles.x == 90)
				rt.eulerAngles = new Vector3 (0, 0, 0);
			else// if (transform.rotation.eulerAngles.x == 180)
				rt.eulerAngles = new Vector3 (0, 90, 0);

			TextMesh tmp = ((GameObject)Instantiate (score, transform.position, rt)).GetComponent<TextMesh> ();
			tmp.text = "Miss!";
			tmp.color = Color.red;
			status.ComboCounter = 0;
			status.MissCount++;
			miss = true;

			if (status.enableSE) {
				missSE.enabled = true;
				missSE.Play ();
			}
			GetComponent<Renderer> ().enabled = false;
		}
	}
}

// H: 73  1.25    96       -83         2      1.75
// leap vector / 50 = unity vector3