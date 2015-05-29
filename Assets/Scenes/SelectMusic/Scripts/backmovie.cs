using UnityEngine;
using System.Collections;

public class backmovie : MonoBehaviour {

	public MovieTexture movTexture;
	
	void Start()
	{
		movTexture = Resources.Load ("lmix") as MovieTexture;
		GetComponent<Renderer>().material.mainTexture  = movTexture;
		
		movTexture.loop = true;
		if(!movTexture.isPlaying)
		{
			movTexture.Play();
		}
	}
}
