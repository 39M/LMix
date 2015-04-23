using UnityEngine;
using System.Collections;

public class NoteGenerator : MonoBehaviour {
	public GameObject note;
	public Vector3 positon;
	public Vector3 rotation;

	// Use this for initialization
	void Start () {
		InvokeRepeating ("GenerateNote", 0, 1);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void GenerateNote()
	{
		Quaternion rt = Quaternion.identity;
		rt.eulerAngles = note.transform.rotation.eulerAngles + rotation;
		GameObject note_tmp = (GameObject) Instantiate (note, positon, rt);
	}
}
