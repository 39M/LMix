using UnityEngine;
using System.Collections;

public class GamePlayer : MonoBehaviour {
	NoteGenerator NGDL;
	NoteGenerator NGDR;
	NoteGenerator NGRU;
	NoteGenerator NGRD;
	NoteGenerator NGLU;
	NoteGenerator NGLD;
	ArrayList beatmap;
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

		time = 0f;
		i = 0;

		/****************
			// Get beatmap from file
		****************/



		// Test
		//NGDL.GenerateNote ();
		//NGLU.GenerateNote ();
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;	// Timing

		if (0 <= i)	// notes count < i
			return;

		if (time >= 1) {	// time > generate time
			switch (1){	// select generator
			case 1:
				NGDL.GenerateNote ();
				break;
			case 2:
				NGDR.GenerateNote ();
				break;
			case 3:
				NGLU.GenerateNote ();
				break;
			case 4:
				NGLD.GenerateNote ();
				break;
			case 5:
				NGRU.GenerateNote ();	
				break;
			case 6:
				NGRD.GenerateNote ();	
				break;
			default:
				break;
			}
			i++;
			//now = beatmap [i] as ArrayList;	// move to next note
		}
	}
}
