using UnityEngine;
using System.Collections;
using System.IO;
using SimpleJSON;
using System.Collections.Generic;
public class GamePlayer : MonoBehaviour
{
	public string beatmapName;	// Unique name of beatmap
	public int difficulty;	// difficulty, 0 - easy, 1 - normal, 2 - hard
	public bool enableSE;
	public GUIText ScoreText;
	public GUIText ComboText;
	public int ScoreCounter;
	public int ComboCounter;
	public int PerfectCount;
	public int GoodCount;
	public int BadCount;
	public int MissCount;
	public bool pause;	// status of music
	public bool stop;	// status of music

	bool loadFail;
	AudioSource music;
	MovieTexture mov;
	NoteGenerator NGDL;
	NoteGenerator NGDR;
	//NoteGenerator NGRU;
	NoteGenerator NGRD;
	//NoteGenerator NGLU;
	NoteGenerator NGLD;
	JSONNode Beatmap;
	JSONArray Objects;	// Objects like note, spinner
	JSONArray now;	// next Object
	//float time;
	int i;
	//bool isPlaying;

	// Use this for initialization
	void Start ()
	{

		/*
			this part used to load seleted song
		 */
/*
{
    "Tags": [],  // 标签
    "Creator": "",  // 制作者
    "GameObject": {  // 游戏物件，如落键，转盘
        "Hard": [],
        "Easy": [[0, 10000, 1, 1],  // [类型, 时间, 位置, 速度]
                 [0, 10000, 2, 1], 
                 [0, 15000, 1, 1], 
                 [0, 16000, 2, 1]],
        "Normal": []
    },
    "Artist": "",  // 艺术家
    "Difficulty": {  // 难度设定
        "Speed": 1
    },
    "Source": "",  // 来源
    "Version": "",  // 版本
    "Title": "",  // 谱面标题
    "Audio": {  // 音频文件信息
        "Length": 0,
        "Name": ""
    }
}
*/
		/*******/
		enableSE = true;

		this.beatmapName = PlayerPrefs.GetString("song");
		Debug.Log (this.beatmapName);
		System.Collections.Generic.Dictionary<string,int> tempset = new System.Collections.Generic.Dictionary<string,int>(){{"Nya",0},{"MirrorNight",2}};
		this.difficulty = tempset[beatmapName];
		/*******/

		//		beatmapName = "Nya";
		//		difficulty = 0;
		//
				beatmapName = "MirrorNight";
				difficulty = 2;

		loadFail = true;	// Asume load fail

		// Get beatmap from file
		TextAsset f = Resources.Load("Music/" + beatmapName + "/beatmap") as TextAsset;
		if (f == null) 	// load fail
			return;

		string s = f.ToString ();
		//Debug.Log (s);
		Beatmap = JSON.Parse (s);
		if (Beatmap == null)	// load fail
			return;

		music = GetComponent<AudioSource> ();
		music.clip = Resources.Load ("Music/" + beatmapName + "/" + Beatmap ["Audio"] ["Name"]) as AudioClip;	// No name-extension
		if (music.clip == null)	// load fail
			return;

		mov = null;
		//mov = (GameObject.Find ("Plane").GetComponent("VideoPlayer") as VideoPlayer).movTexture;

		switch (difficulty) {
		case 0:
			Objects = Beatmap ["GameObject"] ["Easy"].AsArray;
			break;
		case 1:
			Objects = Beatmap ["GameObject"] ["Normal"].AsArray;
			break;
		case 2:
			Objects = Beatmap ["GameObject"] ["Hard"].AsArray;
			break;
		default:
			return;
		}
		if (Objects == null)
			return;

		loadFail = false;	// load success

		NGDL = GameObject.Find ("NoteGeneratorDL").GetComponent ("NoteGenerator") as NoteGenerator;
		NGDR = GameObject.Find ("NoteGeneratorDR").GetComponent ("NoteGenerator") as NoteGenerator;
		//NGRU = GameObject.Find ("NoteGeneratorRU").GetComponent ("NoteGenerator") as NoteGenerator;
		NGRD = GameObject.Find ("NoteGeneratorRD").GetComponent ("NoteGenerator") as NoteGenerator;
		//NGLU = GameObject.Find ("NoteGeneratorLU").GetComponent ("NoteGenerator") as NoteGenerator;
		NGLD = GameObject.Find ("NoteGeneratorLD").GetComponent ("NoteGenerator") as NoteGenerator;

		// Init
		i = 0;
		now = Objects [0].AsArray;
		music.Play ();
		if (mov != null)
			mov.Play ();
		pause = stop = false;
		ScoreCounter = ComboCounter = PerfectCount = GoodCount = BadCount = MissCount = 0;
		ScoreText.text = "Score: " + ScoreCounter.ToString ();
		ComboText.text = "Combo: " + ComboCounter.ToString ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (loadFail || Objects.Count <= i || !music.isPlaying)
			return;

		//Debug.Log (music.time);
		while (Objects.Count > i && music.time >= (now [1].AsFloat - 7 / now [3].AsFloat)) {	// time > generate time
			now = Objects [i].AsArray;	// get current note
			switch (now [2].AsInt) {	// select generator
			case 1:
				NGDL.GenerateNote (1, now [3].AsFloat);
				break;
			case 2:
				NGDR.GenerateNote (1, now [3].AsFloat);
				break;
			case 3:
				//NGLU.GenerateNote (now[3].AsFloat);
				break;
			case 4:
				NGLD.GenerateNote (now[0].AsInt, now [3].AsFloat);
				break;
			case 5:
				//NGRU.GenerateNote (now[3].AsFloat);	
				break;
			case 6:
				NGRD.GenerateNote (now[0].AsInt, now [3].AsFloat);	
				break;
			default:
				break;
			}
			i++;	// move to next note
		}
	}

	void OnGUI ()
	{
		if (GUILayout.Button ("播放/继续")) {
			//播放/继续播放音频
			if (!music.isPlaying) {
				//isPlaying = true;
				pause = stop = false;
				music.Play ();
				if (mov != null)
					mov.Play ();
			}
			
		}
		
		if (GUILayout.Button ("暂停播放")) {
			//暂停播放
			//isPlaying = false;
			pause = true;
			music.Pause ();
			if (mov != null)
				mov.Pause ();
		}
		
		if (GUILayout.Button ("停止播放")) {
			//停止播放
			//isPlaying = false;
			stop = true;
			//time = 0f;
			i = 0;
			now = Objects [0].AsArray;
			music.Stop ();
			if (mov != null)
				mov.Stop ();
			ScoreCounter = ComboCounter = PerfectCount = GoodCount = BadCount = MissCount = 0;
			ScoreText.text = "Score: " + ScoreCounter.ToString ();
			ComboText.text = "Combo: " + ComboCounter.ToString ();
		}
	}
}
