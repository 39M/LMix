using UnityEngine;
using System.Collections;

public class movie : MonoBehaviour {

	// Use this for initialization
	//电影纹理
	public MovieTexture movTexture;
	
	void Start()
	{
		//设置当前对象的主纹理为电影纹理
		GetComponent<Renderer>().material.mainTexture = movTexture;
		//设置电影纹理播放模式为循环
		movTexture.loop = true;
	}
	
	void OnGUI()
	{
		if(GUILayout.Button("播放/继续"))
		{
			//播放/继续播放视频
			if(!movTexture.isPlaying)
			{
				movTexture.Play();
			}
			
		}
		
		if(GUILayout.Button("暂停播放"))
		{
			//暂停播放
			movTexture.Pause();
		}
		
		if(GUILayout.Button("停止播放"))
		{
			//停止播放
			movTexture.Stop();
		}
	}


}


