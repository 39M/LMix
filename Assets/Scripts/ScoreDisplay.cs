using UnityEngine;
using System.Collections;

public class ScoreDisplay : MonoBehaviour {
	float timer;
	// Use this for initialization
	void Start () {
		timer = 0.5f;
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (timer < 0)
			Destroy (gameObject);
	}
}
