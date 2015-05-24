using UnityEngine;
using System.Collections;

public class NoteGenerator : MonoBehaviour {
	public GameObject note;
	public GameObject slidenote;

	/*void Start () {
		//InvokeRepeating ("GenerateNote", 0, 1);
	}*/

	public void GenerateNote(int type, float speed)
	{
		Quaternion rt = Quaternion.identity;
		rt.eulerAngles = note.transform.rotation.eulerAngles + transform.rotation.eulerAngles;
		GameObject note_tmp;

		switch (type) {
		case 0:
			(((GameObject) Instantiate (note, transform.position, rt)).GetComponent ("Drop") as Drop).speed = speed;
			break;
		case 1:
			(((GameObject) Instantiate (slidenote, transform.position, rt)).GetComponent ("SlideDrop") as SlideDrop).speed = speed;
			break;
		default:
			break;
		}
	}
}
