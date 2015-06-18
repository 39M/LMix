using UnityEngine;
using System.Collections;

public class logomov : MonoBehaviour {

	public MovieTexture movTexture;

	// Use this for initialization
	void Start () {
		movTexture = Resources.Load ("logo") as MovieTexture;
		GetComponent<Renderer> ().material.mainTexture = movTexture;

		movTexture.loop = false;
		if (!movTexture.isPlaying) {
			movTexture.Play ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!movTexture.isPlaying) {
			Application.LoadLevel ("SelectMusic");
		}
	}
}
