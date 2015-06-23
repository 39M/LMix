using UnityEngine;
using System.Collections;
using Leap;

public class Spin : MonoBehaviour
{
	public GameObject judgement;
	public float TotalTime = 100000f;
	Color c300, c100, c50, c0;
	GamePlayer status;
	float scorecount = 0;
	float rotate = 0.0f;
	float rotatespeed = 0.0f;
	float maxrotatespeed = 1200f;
	float remaintime = 100000f;
	//CircleGesture circlegesture;
	Vector3 fingeroldposition ;
	protected Controller leap;
	// Use this for initialization
	void Start ()
	{

		//circlegesture = new CircleGesture ();
		leap = new Controller ();
		leap.EnableGesture (Gesture.GestureType.TYPE_CIRCLE);
		Frame fream = leap.Frame ();
		Hand hand = fream.Hands [0];
		Vector3 FingerPos = new Vector3 (0, 0, 0);
		foreach (var finger in hand.Fingers) {

			FingerPos = FingerPos + new Vector3 (finger.TipPosition.x, finger.TipPosition.y, finger.TipPosition.z);

		}
		fingeroldposition = FingerPos;
		remaintime = TotalTime;

		status = GameObject.Find ("GamePlayer").GetComponent ("GamePlayer") as GamePlayer;
		c300 = new Color (58 / 255f, 183 / 255f, 239 / 255f, 0);
		c100 = new Color (191 / 255f, 255 / 255f, 160 / 255f, 0);
		c50 = new Color (251 / 255f, 208 / 255f, 114 / 255f, 0);
		c0 = new Color (249 / 255f, 90 / 255f, 101 / 255f, 0);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (status.pause)
			return;

		// to rotate the spin
		rot ();
		// recalculate the rotate speed 
		Frame fream = leap.Frame ();
		// last version 
		// Hand hand = fream.Hands [0];
		Hand hand = fream.Hands[1];
		if(hand.ToString()=="Invalid Hand"){
			hand = fream.Hands[0];
		}
		Debug.Log("hand: "+hand.ToString());
		Vector3 FingerPos = new Vector3 (0, 0, 0);
		foreach (var finger in hand.Fingers) {
			FingerPos = FingerPos + new Vector3 (finger.TipPosition.x, finger.TipPosition.y, finger.TipPosition.z);
		}
		if (rotatespeed < maxrotatespeed)
			rotatespeed += 0.04f * Time.deltaTime * ((FingerPos - fingeroldposition).sqrMagnitude - 1500.0f);
		if (rotatespeed < 0.0f)
			rotatespeed = 0.0f;
		fingeroldposition = FingerPos;
		remaintime -= Time.deltaTime;
		if (remaintime <= 0.0f) {
			// add score
			// this.rotate is a float that remark the total angle that the spinner has been rotated before it ends; 

			Debug.Log (rotate);

			TextMesh tmp = ((GameObject)Instantiate (judgement, transform.position, Quaternion.identity)).GetComponent<TextMesh> ();
			int ScoreGet;
			if (rotate >= TotalTime / 2f * 360) {
				tmp.text = "Perfect";
				tmp.color = c300;
				ScoreGet = 300 + 300 / 25 * status.ComboCounter;
				status.ComboCounter++;
				status.PerfectCount++;
			} else if (rotate >= TotalTime / 4f * 360) {
				tmp.text = "Good";
				tmp.color = c100;
				ScoreGet = 100 + 100 / 25 * status.ComboCounter;
				status.ComboCounter++;
				status.GoodCount++;
			} else if (rotate >= TotalTime / 8f * 360) {
				tmp.text = "Bad";
				tmp.color = c50;
				ScoreGet = 50 + 50 / 25 * status.ComboCounter;
				status.ComboCounter = 0;
				status.BadCount++;
			} else {
				tmp.text = "Miss";
				tmp.color = c0;
				status.MissCount++;
				ScoreGet = 0;
			}

			status.ScoreCounter += ScoreGet;
			if (status.ComboCounter > status.MaxCombo)
				status.MaxCombo = status.ComboCounter;
			//status.ScoreText.text = "Score: " + status.ScoreNow.ToString ();
			status.ComboText.text = "Combo: " + status.ComboCounter.ToString ();



			// distory this spinner
			Destroy (gameObject);
			//GetComponent<Renderer>().enabled = false;
		}
	}

	void rot ()
	{
		rotate += rotatespeed * Time.deltaTime;
		scorecount += rotatespeed * Time.deltaTime;
		if (scorecount > 1) {
			status.ScoreCounter += (int)scorecount;
			scorecount = 0f;
		}
		Debug.Log (rotate);
		Vector3 rot = new Vector3 ();
		rot = transform.localEulerAngles;
		Debug.Log (rot.ToString ());
		rot.x = rotate;
		rot.y = 90f;
		rot.z = -90f;
		Debug.Log (rot.ToString ());
		transform.localEulerAngles = rot;
	}
}
