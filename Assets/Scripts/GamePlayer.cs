using UnityEngine;
using System.Collections;

public class GamePlayer : MonoBehaviour {
	NoteGenerator NGDL;
	NoteGenerator NGDR;
	NoteGenerator NGRU;
	NoteGenerator NGRD;
	NoteGenerator NGLU;
	NoteGenerator NGLD;
	// Use this for initialization
	void Start () {
		NGDL = GameObject.Find ("NoteGeneratorDL").GetComponent ("NoteGenerator") as NoteGenerator;
		NGDR = GameObject.Find ("NoteGeneratorDR").GetComponent ("NoteGenerator") as NoteGenerator;
		NGRU = GameObject.Find ("NoteGeneratorRU").GetComponent ("NoteGenerator") as NoteGenerator;
		NGRD = GameObject.Find ("NoteGeneratorRD").GetComponent ("NoteGenerator") as NoteGenerator;
		NGLU = GameObject.Find ("NoteGeneratorLU").GetComponent ("NoteGenerator") as NoteGenerator;
		NGLD = GameObject.Find ("NoteGeneratorLD").GetComponent ("NoteGenerator") as NoteGenerator;

		// Test
		NGDL.GenerateNote ();
		NGLU.GenerateNote ();
	}
	
	// Update is called once per frame
	void Update () {

	}
}
