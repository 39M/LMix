using UnityEngine;
using System.Collections;

public class StarMove : MonoBehaviour
{
	GamePlayer status;

	void Start ()
	{
		status = GameObject.Find ("GamePlayer").GetComponent ("GamePlayer") as GamePlayer;
	}

	void Update ()
	{
		if (status.pause)
			return;

		transform.Translate (Vector3.back * Random.Range (5f, 20) * Time.deltaTime, Space.World);
		transform.Rotate (new Vector3 (15, 30, 45) * Time.deltaTime);
		if (transform.position.z < -200)
			Destroy (gameObject);
	}
}
