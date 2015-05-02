using UnityEngine;
using System.Collections;
using SimpleJSON;

public class GamePlayer : MonoBehaviour {
	NoteGenerator NGDL;
	NoteGenerator NGDR;
	NoteGenerator NGRU;
	NoteGenerator NGRD;
	NoteGenerator NGLU;
	NoteGenerator NGLD;
	JSONNode Beatmap;
	JSONArray Easy, Normal, Hard;
	JSONArray now;
	float time;
	int i;

	// Use this for initialization
	void Start () {
		NGDL = GameObject.Find ("NoteGeneratorDL").GetComponent ("NoteGenerator") as NoteGenerator;
		NGDR = GameObject.Find ("NoteGeneratorDR").GetComponent ("NoteGenerator") as NoteGenerator;
		NGRU = GameObject.Find ("NoteGeneratorRU").GetComponent ("NoteGenerator") as NoteGenerator;
		NGRD = GameObject.Find ("NoteGeneratorRD").GetComponent ("NoteGenerator") as NoteGenerator;
		NGLU = GameObject.Find ("NoteGeneratorLU").GetComponent ("NoteGenerator") as NoteGenerator;
		NGLD = GameObject.Find ("NoteGeneratorLD").GetComponent ("NoteGenerator") as NoteGenerator;

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

		string s = @"{
    ""Tags"": [""First Blood""],
    ""Creator"": "",w,"",
    ""GameObject"": {
        ""Hard"": [],
        ""Easy"": [[0, 1, 1, 1],
                 [0, 1, 2, 2],
                 [0, 1, 3, 2],
                 [0, 1, 4, 1]],
        ""Normal"": []
    },
    ""Artist"": ""who"",
    ""Difficulty"": {
        ""Speed"": 1
    },
    ""Source"": ""where"",
    ""Version"": ""1.0"",
    ""Title"": ""title"",
    ""Audio"": {
        ""Length"": 30000,
        ""Name"": ""theaudio""
    }
}";
		/****************
			// Get beatmap from file
		****************/


		Beatmap = JSON.Parse (s);
		Easy = Beatmap["GameObject"]["Easy"].AsArray;

		time = 0f;
		i = 0;
		now = Easy [0].AsArray;
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;	// Timing

		if (Easy.Count <= i)	// notes count < i
			return;

		if (time >= now[1].AsFloat) {	// time > generate time
			switch (now[2].AsInt){	// select generator
			case 1:
				NGDL.GenerateNote (now[3].AsFloat);
				break;
			case 2:
				NGDR.GenerateNote (now[3].AsFloat);
				break;
			case 3:
				NGLU.GenerateNote (now[3].AsFloat);
				break;
			case 4:
				NGLD.GenerateNote (now[3].AsFloat);
				break;
			case 5:
				NGRU.GenerateNote (now[3].AsFloat);	
				break;
			case 6:
				NGRD.GenerateNote (now[3].AsFloat);	
				break;
			default:
				break;
			}
			i++;
			now = Easy[i].AsArray;	// move to next note
		}
	}
}
