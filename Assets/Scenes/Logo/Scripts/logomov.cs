using UnityEngine;
using System.Collections;

public class logomov : MonoBehaviour {

	public MovieTexture movTexture;
	public AudioClip audioclip;

	// Use this for initialization
	void Start () {
		movTexture = Resources.Load ("logomov") as MovieTexture;
		audioclip = Resources.Load ("logowav") as AudioClip;
		GetComponent<Renderer> ().material.mainTexture = movTexture;
		GetComponent<AudioSource> ().clip = audioclip;

		movTexture.loop = false;
		if (!movTexture.isPlaying) {
			movTexture.Play ();
			GetComponent<AudioSource> ().Play();	
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!movTexture.isPlaying) {
			Application.LoadLevel ("SelectMusic");
		}
	}
}
