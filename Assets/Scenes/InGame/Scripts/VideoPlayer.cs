using UnityEngine;
using System.Collections;

public class VideoPlayer : MonoBehaviour
{
	public MovieTexture movTexture = null;
	
	void Start ()
	{
		if ((GameObject.Find ("GamePlayer").GetComponent ("GamePlayer") as GamePlayer).enableBG) {
			GetComponent<Renderer> ().material.mainTexture = movTexture;
			movTexture.loop = true;
		}
	}
}


