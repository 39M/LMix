using UnityEngine;
using System.Collections;
using Leap;
public class Spin : MonoBehaviour {

	float rotate = 0.0f;
	float rotatespeed = 20.0f;
	float remaintime = 3.0f;
	CircleGesture circlegesture;
	Vector3 fingeroldposition ;
	protected Controller leap;
	// Use this for initialization
	void Start () {

		circlegesture = new CircleGesture();
		leap = new Controller();
		leap.EnableGesture(Gesture.GestureType.TYPE_CIRCLE);
		Frame fream = leap.Frame ();
		Hand hand = fream.Hands [0];
		Vector3 FingerPos = new Vector3(0,0,0);
		foreach (var finger in hand.Fingers) {

			FingerPos=FingerPos +  new Vector3(finger.TipPosition.x,finger.TipPosition.y,finger.TipPosition.z);

		}
		fingeroldposition = FingerPos;
	}
	
	// Update is called once per frame
	void Update () {
		// to rotate the spin
		rot ();
		// recalculate the rotate speed 
		Frame fream = leap.Frame ();
		Hand hand = fream.Hands [0];
		Vector3 FingerPos = new Vector3(0,0,0);
		foreach (var finger in hand.Fingers) {
			FingerPos =FingerPos +  new Vector3(finger.TipPosition.x,finger.TipPosition.y,finger.TipPosition.z);
		}
		rotatespeed =Time.deltaTime *(FingerPos - fingeroldposition).sqrMagnitude;
		fingeroldposition = FingerPos;
		remaintime-=Time.deltaTime;
		if(remaintime <= 0.0f){
			// add score
			// this.rotate is a float that remark the total angle that the spinner has been rotated before it ends; 

			// distory this spinner
			GetComponent<Renderer>().enabled = false;
		}
	}
	void rot(){
		rotate += rotatespeed * Time.deltaTime;
		Debug.Log(rotate);
		Vector3 rot = new Vector3();
		rot = transform.localEulerAngles;
		Debug.Log(rot.ToString());
		rot.x =rotate;
		rot.y = 90f;
		rot.z = 90f;
		Debug.Log(rot.ToString());
		transform.localEulerAngles = rot;
	}
}
