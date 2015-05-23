using UnityEngine;
using System.Collections;

public class VideoPlayer : MonoBehaviour {

	// Use this for initialization
	//电影纹理
	public MovieTexture movTexture;
	
	void Start()
	{
		//设置当前对象的主纹理为电影纹理
		GetComponent<Renderer>().material.mainTexture  = movTexture;
		//设置电影纹理播放模式为循环
		movTexture.loop = true;
		if(!movTexture.isPlaying)
		{
			movTexture.Play();
		}
	}
}


