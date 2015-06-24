using UnityEngine;
using System.Collections;

public class VideoPlayer : MonoBehaviour
{
	public MovieTexture movTexture = null;
	
	void Start ()
	{
		if ((GameObject.Find ("GamePlayer").GetComponent ("GamePlayer") as GamePlayer).enableBG) {
			if (movTexture == null) {
				(GameObject.Find ("GamePlayer").GetComponent ("GamePlayer") as GamePlayer).enableBG = false;
				Destroy(gameObject);
				return;
			}
			GetComponent<Renderer> ().material.mainTexture = movTexture;
			movTexture.loop = true;
		}
	}
}


