using UnityEngine;
using System.Collections;

public class VideoPlayer : MonoBehaviour
{
	public MovieTexture movTexture = null;
	
	void Start ()
	{
		GetComponent<Renderer> ().material.mainTexture = movTexture;
		if ((GameObject.Find ("GamePlayer").GetComponent ("GamePlayer") as GamePlayer).enableBG)
			movTexture.loop = true;
	}
}


