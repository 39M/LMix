using UnityEngine;
using System.Collections;

public class NoteGenerator : MonoBehaviour {
	public GameObject note;
	//public Vector3 positon;
	//public Vector3 rotation;

	// Use this for initialization
	void Start () {
		//InvokeRepeating ("GenerateNote", 0, 1);
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void GenerateNote(float speed)
	{
		Quaternion rt = Quaternion.identity;
		rt.eulerAngles = note.transform.rotation.eulerAngles + transform.rotation.eulerAngles;
		GameObject note_tmp = (GameObject) Instantiate (note, transform.position, rt);
		Drop temp = note_tmp.GetComponent ("Drop") as Drop;
		temp.speed = speed;
	}
}
