using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
	public Image Background;
	public Text ScoreText;
	public Text ComboText;
	public Text PerfectText;
	public Text GoodText;
	public Text BadText;
	public Text MissText;
	public Text Judgement;
	public Button RetryButton;
	public Button BackButton;
	// Use this for initialization
	void Start ()
	{
		//var a = Background.GetComponent<RectTransform> ();

		//= Screen.width;
		//Background.minHeight = Screen.height;
		ScoreText.text = "Score\t\t\t" + PlayerPrefs.GetInt ("ScoreCount");
		ComboText.text = "Combo\t\t" + PlayerPrefs.GetInt ("ComboCount");
		PerfectText.text = "Perfect\t\t" + PlayerPrefs.GetInt ("PerfectCount");
		GoodText.text = "Good\t\t\t" + PlayerPrefs.GetInt ("GoodCount");
		BadText.text = "Bad\t\t\t\t" + PlayerPrefs.GetInt ("BadCount");
		MissText.text = "Miss\t\t\t" + PlayerPrefs.GetInt ("MissCount");

		RetryButton.onClick.AddListener (Retry);
		BackButton.onClick.AddListener (Back);
	}
	
	// Update is called once per frame
	void Update ()
	{
		/*
		 PlayerPrefs.SetInt("ScoreCount", ScoreCounter);
				PlayerPrefs.SetInt("ComboCount", ComboCounter);
				PlayerPrefs.SetInt("PerfectCount", PerfectCount);
				PlayerPrefs.SetInt("GoodCount", GoodCount);
				PlayerPrefs.SetInt("BadCount", BadCount);
				PlayerPrefs.SetInt("MissCount", MissCount);*/
	}

	void Retry ()
	{
		Application.LoadLevel ("InGame");
	}

	void Back ()
	{
		Application.LoadLevel ("SelectMusic");
	}
}
