using UnityEngine;
using System.Collections;

public class SpaceGenerator : MonoBehaviour
{
	public GameObject star;
	GamePlayer status;

	void Start ()
	{
		status = GameObject.Find ("GamePlayer").GetComponent ("GamePlayer") as GamePlayer;
	}

	void Update ()
	{
		if (status.pause)
			return;

		if (Random.Range (0, 100) > 50)
			GenerateStar ();
	}

	void GenerateStar ()
	{
		Vector3 pos = new Vector3 (Random.Range (-48, 36), Random.Range (-48, 48), 200);
		if (Mathf.Abs (pos.x) <= 3)
			pos.x *= 3;
		if (Mathf.Abs (pos.x) == 0)
			pos.x = -3;
		if (Mathf.Abs (pos.y) <= 3)
			pos.y *= 3;
		if (Mathf.Abs (pos.y) == 0)
			pos.y = -3;
		Instantiate (star, pos, star.transform.rotation);
	}
}
