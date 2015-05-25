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
	//bool miss;
	bool hitable;
	Color c300, c100, c50, c0;

	void Start ()
	{
		leap = new Controller ();
		hit = pause = stop = hitable = false;

		status = GameObject.Find ("GamePlayer").GetComponent ("GamePlayer") as GamePlayer;

		hitSE = GetComponent<AudioSource> ();
		if (!status.defaultSE)
			hitSE.clip = Resources.Load ("Music/" + status.beatmapName + "/" + status.SEname ["hit"]) as AudioClip;
		else
			hitSE.clip = Resources.Load ("Default/hit") as AudioClip;
		missSE = hitSE;
		if (!status.enableSE)
			hitSE.mute = true;

		c300 = new Color (58 / 255f, 183 / 255f, 239 / 255f);
		c100 = new Color(191 / 255f, 255 / 255f, 160 / 255f);
		c50 = new Color(251 / 255f, 208 / 255f, 114 / 255f);
		c0 = new Color(249 / 255f, 90 / 255f, 101 / 255f);

		int width = 200, y_base = 0;
		float y_basef = 1.15f;
		lim_z_low = -175;
		lim_z_high = 175;

		notePos = transform.position;
		if (notePos.x > 0) {
			if (notePos.y < y_basef - 0.5) {
				lim_x_low = 0;
				lim_x_high = 200;
				lim_y_low = y_base - 80;
				lim_y_high = lim_y_low + width;	// 80
			} else if (notePos.y < y_basef + 2) {
				lim_x_low = 170;
				lim_x_high = 300;
				lim_y_low = y_base + 50;
				lim_y_high = lim_y_low + width;
			} else {
				lim_x_low = 170;
				lim_x_high = 300;
				lim_y_low = 250;
				lim_y_high = 450;
			}
		} else {
			if (notePos.y < y_basef - 0.5) {
				lim_x_low = -200;
				lim_x_high = 0;
				lim_y_low = y_base - 80;
				lim_y_high = lim_y_low + width;	// 80
			} else if (notePos.y < y_basef + 2) {
				lim_x_low = -300;
				lim_x_high = -170;
				lim_y_low = y_base + 50;
				lim_y_high = lim_y_low + width;
			} else {
				lim_x_low = -300;
				lim_x_high = -170;
				lim_y_low = 250;
				lim_y_high = 450;
			}
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
		if (!hit && notePos.z < 2 && notePos.z > -1.75) {
			Frame frame = leap.Frame ();
			foreach (Hand hand in frame.Hands) {
				// Debug.Log (hand.Fingers.Frontmost.TipPosition);
				foreach (Finger finger in hand.Fingers) {
					FingerPos = finger.TipPosition;
					// Destroy note if tap success
					if (lim_y_low < FingerPos.y && FingerPos.y < lim_y_high && 
						lim_x_low < FingerPos.x && FingerPos.x < lim_x_high && 
						lim_z_low < FingerPos.z && FingerPos.z < lim_z_high) {
						if (!hitable)
							return;

						Quaternion rt = Quaternion.identity;
						if (transform.rotation.eulerAngles.x == 0)
							rt.eulerAngles = new Vector3 (0, -90, 0);
						else if (transform.rotation.eulerAngles.x == 90)
							rt.eulerAngles = new Vector3 (0, 0, 0);
						else// if (transform.rotation.eulerAngles.x == 180)
							rt.eulerAngles = new Vector3 (0, 90, 0);
						//Debug.Log(transform.rotation.eulerAngles.x / 2);
						TextMesh tmp = ((GameObject)Instantiate (score, transform.position, rt)).GetComponent<TextMesh> ();
						if (Mathf.Abs (notePos.z) < 0.75) {
							tmp.text = "Perfect!";
							tmp.color = c300;
							status.ComboCounter++;
							status.PerfectCount++;
							status.ScoreCounter += 300 * status.ComboCounter;
						} else if (Mathf.Abs (notePos.z) < 1.5) {
							tmp.text = "Good!";
							tmp.color = c100;
							status.ComboCounter++;
							status.GoodCount++;
							status.ScoreCounter += 100 * status.ComboCounter;
						} else if (Mathf.Abs (notePos.z) < 1.75) {
							tmp.text = "Bad!";
							tmp.color = c50;
							status.ComboCounter = 0;
							status.BadCount++;
							status.ScoreCounter += 50 * status.ComboCounter;
						} else {
							tmp.text = "Miss!";
							tmp.color = c0;
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
			hitable = true;
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
			tmp.color = c0;
			status.ComboCounter = 0;
			status.MissCount++;
			status.ComboText.text = "Combo: " + status.ComboCounter.ToString ();
			//miss = true;

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