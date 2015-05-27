using UnityEngine;
using System.Collections;
using Leap;
public class Spin : MonoBehaviour {

	float rotate = 0.0f;
	float rotatespeed = 0.0f;
	protected Controller leap;
	// Use this for initialization
	void Start () {
		leap = new Controller();
	}
	
	// Update is called once per frame
	void Update () {
		// to rotate the spin
		rot ();
		// recalculate the rotate speed 
		Frame fream = leap.Frame ();
		Hand hand = fream.Hands [0];

	}
	void rot(){
		rotate += rotatespeed * Time.deltaTime;
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
