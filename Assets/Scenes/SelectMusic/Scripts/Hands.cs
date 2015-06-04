using UnityEngine;
using System.Collections;
using Leap;

public class Hands : MonoBehaviour
{
	Controller leap;
	// Use this for initialization
	void Start ()
	{
		leap = new Controller ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		Frame fream = leap.Frame ();
		Hand hand = fream.Hands [0];
		Vector3 pos = new Vector3 (hand.WristPosition.x / 100.0f, hand.WristPosition.y / 100.0f - 0.7f, 5);
		this.transform.position = pos;
		//Debug.Log ("("+hand.WristPosition.x+","+hand.WristPosition.y+")");
	}
}
