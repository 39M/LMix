using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour {
	AudioSource music;
	// Use this for initialization
	void Start () {
		music = GetComponent<AudioSource> ();
		//listener = GetComponent<AudioListener> ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnGUI() {
		if(GUILayout.Button("播放/继续"))
		{
			//播放/继续播放音频
			if(!music.isPlaying)
			{
				music.Play();
			}
			
		}
		
		if(GUILayout.Button("暂停播放"))
		{
			//暂停播放
			music.Pause();
		}
		
		if(GUILayout.Button("停止播放"))
		{
			//停止播放
			music.Stop ();
		}
	}
}
