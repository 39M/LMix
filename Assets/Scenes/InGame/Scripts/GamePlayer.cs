using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using SimpleJSON;
using System.Collections.Generic;

public class GamePlayer : MonoBehaviour
{
	public string beatmapName;	// Unique name of beatmap
	public int difficulty;	// difficulty, 0 - easy, 1 - normal, 2 - hard
	public bool enableSE;
	public bool defaultSE;
	public bool enableBG;
	bool useMovBG;
	bool usePicBG;
	bool defaultBG;
	public JSONNode SEname;
	public Text ScoreText;
	public Text ComboText;
	public Button PauseButton;
	public Text PauseButtonText;
	public Button StopButton;
	public Text StopButtonText;
	public long ScoreCounter;
	public long ScoreNow;
	public int ComboCounter;
	public int MaxCombo;
	public int PerfectCount;
	public int GoodCount;
	public int BadCount;
	public int MissCount;
	public bool pause;	// status of music
	public bool[] trackbusy = {false, false, false, false};
	TextMesh CoverMesh;
	Color CoverColor;
	float CoverTimer;
	bool CoverDone;
	bool loadFail;
	bool gameover;
	float gameoverTimer;
	AudioSource music;
	MovieTexture mov;
	NoteGenerator NGDL;
	NoteGenerator NGDR;
	//NoteGenerator NGRU;
	NoteGenerator NGRD;
	//NoteGenerator NGLU;
	NoteGenerator NGLD;
	SpinnerGenerator SG;
	TapDetector TD;
	JSONNode Beatmap;
	JSONArray HitObjects;	// HitObjects like note, spinner
	JSONArray now;	// next Object

	int i;	//note counter\

	int NotesBeforeMusic;
	bool NotesBeforeDone;
	float Timer;
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
    "Background": {	// 背景信息
        "Enable": false,	// 是否启用
        "Type": "",	// 类型 0-图片 1-视频
        "Name": ""	// 文件名
    }
    "SoundEffect": {
    	"Enable": false,
    	"SE": []
    }
}
*/
		/*******/
		PlayerPrefs.SetInt ("GameStarted", 1);
		enableSE = PlayerPrefs.GetInt ("enableSE") != 0;
		enableBG = PlayerPrefs.GetInt ("enableBG") != 0;

		this.beatmapName = PlayerPrefs.GetString ("song");
		Debug.Log (" Play " + this.beatmapName);
		this.difficulty = PlayerPrefs.GetInt ("Difficulty");
		/*******/

		//beatmapName = "Nya";
		//difficulty = 0;
		//beatmapName = "MirrorNight";
		//difficulty = 2;
		//beatmapName = "LetItGo";
		//difficulty = 2;

		loadFail = true;	// Asume load fail

		// Get beatmap from file
		TextAsset f = Resources.Load ("Music/" + beatmapName + "/beatmap") as TextAsset;
		if (f == null) 	// load fail
			return;
		Debug.Log ("Beatmap load success");

		string s = f.ToString ();
		//Debug.Log (s);
		Beatmap = JSON.Parse (s);
		if (Beatmap == null)	// load fail
			return;
		Debug.Log ("Beatmap parse success");

		music = GetComponent<AudioSource> ();
		music.clip = Resources.Load ("Music/" + beatmapName + "/" + Beatmap ["Audio"] ["Name"]) as AudioClip;	// No name-extension
		if (music.clip == null)	// load fail
			return;
		Debug.Log ("audio load success");

		if (Beatmap ["SoundEffect"] ["Enable"].AsBool) {
			defaultSE = false;
			SEname = Beatmap ["SoundEffect"] ["Name"];
			Debug.Log ("use custom SE");
		} else {
			defaultSE = true;
			Debug.Log ("use default SE");
		}

		//defaultBG = true;
		useMovBG = false;
		usePicBG = false;
		if (enableBG) {
			if (Beatmap ["Background"] ["Enable"].AsBool) {
				Destroy (GameObject.Find ("SpaceGenetator"));
				//defaultBG = false;
				switch (Beatmap ["Background"] ["Type"].AsInt) {
				case 0:
				// use Picture as background
					usePicBG = true;
					Debug.Log ("use custom pic BG");
					break;
				case 1:
					// use video as background
					mov = Resources.Load ("Music/" + beatmapName + "/" + Beatmap ["Background"] ["Name"]) as MovieTexture;
					if (mov == null)	// load fail
						return;
					(GameObject.Find ("Backgound").GetComponent ("VideoPlayer") as VideoPlayer).movTexture = mov;
					useMovBG = true;
					Debug.Log ("use custom mov BG");
					break;
				default:
					break;
				}
			} else {
				// use default space background
				Destroy (GameObject.Find ("Backgound"));
				Debug.Log ("use default BG");
			}
		} else {
			Destroy (GameObject.Find ("Backgound"));
			Destroy (GameObject.Find ("SpaceGenetator"));
			Debug.Log ("not use BG");
		}
		Debug.Log ("background load success");

		switch (difficulty) {
		case 0:
			HitObjects = Beatmap ["GameObject"] ["Easy"].AsArray;
			break;
		case 1:
			HitObjects = Beatmap ["GameObject"] ["Normal"].AsArray;
			break;
		case 2:
			HitObjects = Beatmap ["GameObject"] ["Hard"].AsArray;
			break;
		default:
			return;
		}
		if (HitObjects == null)
			return;
		Debug.Log ("hitObjects load success");

		CoverMesh = GameObject.Find ("Cover").GetComponent<TextMesh> ();
		CoverColor = new Color (CoverMesh.color.r, CoverMesh.color.g, CoverMesh.color.b, 1);
		CoverTimer = 0.5f;
		CoverDone = false;


		loadFail = false;	// load success
		Debug.Log ("load success");

		NGDL = GameObject.Find ("NoteGeneratorDL").GetComponent ("NoteGenerator") as NoteGenerator;
		NGDR = GameObject.Find ("NoteGeneratorDR").GetComponent ("NoteGenerator") as NoteGenerator;
		//NGRU = GameObject.Find ("NoteGeneratorRU").GetComponent ("NoteGenerator") as NoteGenerator;
		NGRD = GameObject.Find ("NoteGeneratorRD").GetComponent ("NoteGenerator") as NoteGenerator;
		//NGLU = GameObject.Find ("NoteGeneratorLU").GetComponent ("NoteGenerator") as NoteGenerator;
		NGLD = GameObject.Find ("NoteGeneratorLD").GetComponent ("NoteGenerator") as NoteGenerator;
		SG = GameObject.Find ("SpinnerGenerator").GetComponent ("SpinnerGenerator") as SpinnerGenerator;
		TD = GameObject.Find ("TapDetector").GetComponent ("TapDetector") as TapDetector;

		// Init
		i = 0;
		now = HitObjects [0].AsArray;
		pause = gameover = false;
		ScoreCounter = ComboCounter = MaxCombo = PerfectCount = GoodCount = BadCount = MissCount = 0;
		ScoreNow = 0;
		ScoreText.text = "Score: " + ScoreCounter.ToString ();
		ComboText.text = "Combo: " + ComboCounter.ToString ();
		ScoreText.fontSize = ComboText.fontSize = (int)(Screen.width * 0.03f);

		PauseButton.onClick.AddListener (PauseResume);
		StopButton.onClick.AddListener (StopGame);
		
		Timer = 0f;
		NotesBeforeDone = false;

		if (now [1].AsFloat - 7 / now [3].AsFloat < 0) {
			Timer = Mathf.Abs (now [1].AsFloat - 7 / now [3].AsFloat);
			NotesBeforeDone = false;
		} else {
			StartGame ();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (loadFail)
			return;

		if (TD.PauseTrigger)
			PauseResume ();
		
		if (TD.Exit)
			StopGame ();

		if (!pause) {
			if (ScoreNow < ScoreCounter)
			if (ScoreCounter - ScoreNow < 10)
				ScoreNow++;
			else
				ScoreNow += (ScoreCounter - ScoreNow) / 10;
			ScoreText.text = "Score: " + ScoreNow.ToString ();
		} else {
			if (!TD.Exit)
				return;
		}

		if (HitObjects.Count <= i) {
			if (!gameover) {
				gameover = true;
				if (now [0].AsInt == 3)
					gameoverTimer = 4f + now [3].AsFloat;
				else
					gameoverTimer = 4f + 7 / now [3].AsFloat;

				if (TD.Exit)
					gameoverTimer = 0.5f;

				CoverDone = false;
			}

			gameoverTimer -= Time.deltaTime;
			if (gameoverTimer < 4 && !TD.Exit)
				music.volume -= Time.deltaTime / 4f;

			if (gameoverTimer < 0.5 && !TD.Exit)
				music.volume -= Time.deltaTime * 2;

			if (gameoverTimer < 0.5) {
				CoverColor.a += Time.deltaTime * 2;
				CoverMesh.color = CoverColor;
			}

			if (gameoverTimer < 0) {
				if (TD.Exit) {
					Application.LoadLevel ("SelectMusic");
					Camera.main.GetComponent<AudioListener>().enabled = false;
					return;
				}

				PlayerPrefs.SetString ("ScoreCount", ScoreCounter.ToString ());
				PlayerPrefs.SetInt ("ComboCount", MaxCombo);
				PlayerPrefs.SetInt ("PerfectCount", PerfectCount);
				PlayerPrefs.SetInt ("GoodCount", GoodCount);
				PlayerPrefs.SetInt ("BadCount", BadCount);
				PlayerPrefs.SetInt ("MissCount", MissCount);

				if (PlayerPrefs.GetString (beatmapName + "MaxScore").Equals (""))
					PlayerPrefs.SetString (beatmapName + "MaxScore", ScoreCounter.ToString ());
				else if (long.Parse (PlayerPrefs.GetString (beatmapName + "MaxScore")) < ScoreCounter)
					PlayerPrefs.SetString (beatmapName + "MaxScore", ScoreCounter.ToString ());

				if (PerfectCount == HitObjects.Count)
					PlayerPrefs.SetString ("Judgement", "X");
				else if (PerfectCount >= HitObjects.Count * 0.9f)
					PlayerPrefs.SetString ("Judgement", "S");
				else if (PerfectCount >= HitObjects.Count * 0.75f)
					PlayerPrefs.SetString ("Judgement", "A");
				else if (PerfectCount >= HitObjects.Count * 0.6f)
					PlayerPrefs.SetString ("Judgement", "B");
				else if (PerfectCount >= HitObjects.Count * 0.5f)
					PlayerPrefs.SetString ("Judgement", "C");
				else
					PlayerPrefs.SetString ("Judgement", "D");

				Application.LoadLevel ("GameOver");
				Camera.main.GetComponent<AudioListener>().enabled = false;
			}
			return;
		}


		if (!NotesBeforeDone) {
			Timer -= Time.deltaTime;
			if (Timer <= 0) {
				NotesBeforeDone = true;
				Timer = 0;
				StartGame ();
			}
		}

		if (!CoverDone) {
			CoverColor.a -= Time.deltaTime * 2;
			CoverMesh.color = CoverColor;
			CoverTimer -= Time.deltaTime;
			if (CoverTimer < 0)
				CoverDone = true;
		}

		//Debug.Log (music.time);
		switch (now [0].AsInt) {
		case 3:	// Generate Spinner
			if (HitObjects.Count > i && music.time >= now [1].AsFloat) {
				//SG.Generate (now [3].AsFloat);
				i++;	// move to next note
				if (HitObjects.Count > i)
					now = HitObjects [i].AsArray;	// get current note
			}
			break;
		default:	// Generate Note
			//Debug.Log(HitObjects.Count);
			while (HitObjects.Count > i && music.time - Timer >= (now [1].AsFloat - 7 / now [3].AsFloat)) {	// time > generate time
				switch (now [2].AsInt) {	// select generator
				case 1:
					//NGDL.GenerateNote (now [0].AsInt, now [3].AsFloat);
					NGDL.GenerateNote (1, now [3].AsFloat);
					break;
				case 2:
					//NGDR.GenerateNote (now [0].AsInt, now [3].AsFloat);
					NGDR.GenerateNote (1, now [3].AsFloat);
					break;
				case 3:
					//NGLU.GenerateNote (now[3].AsFloat);
					break;
				case 4:
					//NGLD.GenerateNote (now [0].AsInt, now [3].AsFloat);
					NGLD.GenerateNote (1, now [3].AsFloat);
					break;
				case 5:
					//NGRU.GenerateNote (now[3].AsFloat);	
					break;
				case 6:
					//NGRD.GenerateNote (now [0].AsInt, now [3].AsFloat);	
					NGRD.GenerateNote (1, now [3].AsFloat);	
					break;
				default:
					break;
				}
				i++;	// move to next note
				if (HitObjects.Count > i)
					now = HitObjects [i].AsArray;	// get current note
			}
			break;
		}
	}

	void StartGame ()
	{
		music.Play ();
		if (useMovBG)
			mov.Play ();
	}

	void PauseResume ()
	{
		TD.PauseTrigger = false;
		if (!music.isPlaying) {
			pause = false;
			music.Play ();
			if (useMovBG)
				mov.Play ();
			PauseButtonText.text = "Pause";
		} else {
			pause = true;
			music.Pause ();
			if (useMovBG)
				mov.Pause ();
			PauseButtonText.text = "Resume";
		}
	}

	void StopGame ()
	{
		//Application.LoadLevel ("SelectMusic");
		i = HitObjects.Count;
		TD.Exit = true;
	}
}
