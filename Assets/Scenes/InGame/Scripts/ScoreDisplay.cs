using UnityEngine;
using System.Collections;

public class ScoreDisplay : MonoBehaviour {
	float timer;
	GamePlayer status;
	TextMesh tm;
	Color c;
	// Use this for initialization
	void Start () {
		status = GameObject.Find ("GamePlayer").GetComponent ("GamePlayer") as GamePlayer;
		tm = gameObject.GetComponent<TextMesh> ();
		c = new Color(tm.color.r, tm.color.g, tm.color.b, 0);
		timer = 1f;
	}
	
	// Update is called once per frame
	void Update () {
		if (status.pause)
			return;

		if (timer > 0.9f) {
			c.a += Time.deltaTime * 10;
			tm.color = c;
		}

		if (timer < 0.4f) {
			c.a -= Time.deltaTime * 3f;
			tm.color = c;
		}


		timer -= Time.deltaTime;

		if (timer < 0)
			Destroy (gameObject);
	}
}
