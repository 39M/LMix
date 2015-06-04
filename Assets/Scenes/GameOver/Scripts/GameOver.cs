using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
	public Image Background;
	public Text ScoreLabel;
	public Text ComboLabel;
	public Text PerfectLabel;
	public Text GoodLabel;
	public Text BadLabel;
	public Text MissLabel;
	public Text ScoreText;
	public Text ComboText;
	public Text PerfectText;
	public Text GoodText;
	public Text BadText;
	public Text MissText;
	public Text Judgement;
	public Button RetryButton;
	public Text RetryButtonText;
	public Button BackButton;
	public Text BackButtonText;
	public Image BigCover;
	public Image SmallCover;
	public Image EndCover;
	Leap.Controller leap;
	long TotalScore;
	long ScoreNow;
	int MaxCombo;
	int ComboNow;
	int PerfectCount;
	int PerfectNow;
	int GoodCount;
	int GoodNow;
	int BadCount;
	int BadNow;
	int MissCount;
	int MissNow;
	bool ShowUI;
	bool DisplayDone;
	Color CoverColor;
	float Timer;
	bool Retrying = false;
	bool Backing = false;
	bool RBDone = false;

	// Use this for initialization
	void Start ()
	{
		TotalScore = long.Parse (PlayerPrefs.GetString ("ScoreCount")); 
		MaxCombo = PlayerPrefs.GetInt ("ComboCount");
		PerfectCount = PlayerPrefs.GetInt ("PerfectCount");
		GoodCount = PlayerPrefs.GetInt ("GoodCount");
		BadCount = PlayerPrefs.GetInt ("BadCount");
		MissCount = PlayerPrefs.GetInt ("MissCount");
		Judgement.text = PlayerPrefs.GetString ("Judgement");

		if (Judgement.text == "A")
			Judgement.color = new Color (58 / 255f, 183 / 255f, 239 / 255f);
		else if (Judgement.text == "B")
			Judgement.color = new Color (191 / 255f, 255 / 255f, 160 / 255f);
		else if (Judgement.text == "C")
			Judgement.color = new Color (251 / 255f, 208 / 255f, 114 / 255f);
		else if (Judgement.text == "D")
			Judgement.color = new Color (249 / 255f, 90 / 255f, 101 / 255f);


		ScoreNow = ComboNow = PerfectNow = GoodNow = BadNow = MissNow = 0;
		ShowUI = false;

		CoverColor = new Color (BigCover.color.r, BigCover.color.g, BigCover.color.b, 1);
		
		RetryButton.onClick.AddListener (Retry);
		BackButton.onClick.AddListener (Back);

		ScoreLabel.fontSize = ComboLabel.fontSize = 
		PerfectLabel.fontSize = GoodLabel.fontSize = BadLabel.fontSize = MissLabel.fontSize =
		ScoreText.fontSize = ComboText.fontSize = 
		PerfectText.fontSize = GoodText.fontSize = BadText.fontSize = MissText.fontSize = (int)(Screen.width / 22.5f);
		Judgement.fontSize = (int)(Screen.width * 0.35f);
		RetryButtonText.fontSize = BackButtonText.fontSize = (int)(Screen.width / 50f);


		float pos_x = -Screen.width / 3f, pos_y = Screen.height / 40f, pos_xx = pos_x + Screen.width / 4.5f;
		ScoreLabel.rectTransform.anchoredPosition = new Vector2 (pos_x, pos_y * 13);
		ComboLabel.rectTransform.anchoredPosition = new Vector2 (pos_x, pos_y * 9);
		PerfectLabel.rectTransform.anchoredPosition = new Vector2 (pos_x, pos_y * 3);
		GoodLabel.rectTransform.anchoredPosition = new Vector2 (pos_x, -pos_y * 1);
		BadLabel.rectTransform.anchoredPosition = new Vector2 (pos_x, -pos_y * 5);
		MissLabel.rectTransform.anchoredPosition = new Vector2 (pos_x, -pos_y * 9);

		ScoreText.rectTransform.anchoredPosition = new Vector2 (pos_xx, pos_y * 13);
		ComboText.rectTransform.anchoredPosition = new Vector2 (pos_xx, pos_y * 9);
		PerfectText.rectTransform.anchoredPosition = new Vector2 (pos_xx, pos_y * 3);
		GoodText.rectTransform.anchoredPosition = new Vector2 (pos_xx, -pos_y * 1);
		BadText.rectTransform.anchoredPosition = new Vector2 (pos_xx, -pos_y * 5);
		MissText.rectTransform.anchoredPosition = new Vector2 (pos_xx, -pos_y * 9);

		Judgement.rectTransform.anchoredPosition = new Vector2 (Screen.width / 4f, Screen.height / 10f);
		RetryButton.image.rectTransform.anchoredPosition = new Vector2 (Screen.width / 4f - Screen.width / 15f, -pos_y * 10f);
		BackButton.image.rectTransform.anchoredPosition = new Vector2 (Screen.width / 4f + Screen.width / 15f, -pos_y * 10f);
		RetryButton.image.rectTransform.sizeDelta = BackButton.image.rectTransform.sizeDelta = new Vector2 (Screen.width / 10f, Screen.width / 10f / 2.75f);

		SmallCover.rectTransform.sizeDelta = new Vector2 (Screen.width * 0.8f, Screen.height);

		Timer = 10f;

		leap = new Leap.Controller ();

		leap.EnableGesture (Leap.Gesture.GestureType.TYPE_SWIPE);
		//leap.Config.SetFloat ("Gesture.Swipe.MinLength", 200.0f);
		//leap.Config.SetFloat ("Gesture.Swipe.MinVelocity", 1050f);
		//leap.Config.Save ();

		/*TotalScore = 1000000;
		MaxCombo = 512;
		PerfectCount = 500;
		GoodCount = 200;
		BadCount = 10;
		MissCount = 5;
		Judgement.text = "S";*/
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (RBDone) {
			if (Retrying)
				Application.LoadLevel ("InGame");
			else
				Application.LoadLevel ("SelectMusic");
			return;
		}

		if (Retrying || Backing) {
			if (CoverColor.a < 0) {
				Debug.Log(1);
				//BigCover = Instantiate (CoverPrefab);
				EndCover.rectTransform.sizeDelta = new Vector2 (Screen.width * 2, Screen.height * 2);
				CoverColor = new Color (EndCover.color.r, EndCover.color.g, EndCover.color.b, 0);
			} else {
				Debug.Log(2);
				CoverColor.a += Time.deltaTime * 2f;
				EndCover.color = CoverColor;
				if (CoverColor.a >= 1)
					RBDone = true;
			}
			return;
		}


		if (!ShowUI) {
			CoverColor.a -= Time.deltaTime * 2;
			BigCover.color = CoverColor;
			if (CoverColor.a <= 0) {
				ShowUI = true;
				CoverColor.a = 1f;
				Destroy (BigCover);
			}
		}

		DisplayDone = true;
		CounterPlus (ref ScoreNow, TotalScore, 10);
		CounterPlus (ref ComboNow, MaxCombo, 20);
		CounterPlus (ref PerfectNow, PerfectCount, 20);
		CounterPlus (ref GoodNow, GoodCount, 20);
		CounterPlus (ref BadNow, BadCount, 20);
		CounterPlus (ref MissNow, MissCount, 20);

		if (!DisplayDone) {
			ScoreText.text = ScoreNow.ToString ();
			ComboText.text = ComboNow.ToString ();
			PerfectText.text = PerfectNow.ToString ();
			GoodText.text = GoodNow.ToString ();
			BadText.text = BadNow.ToString ();
			MissText.text = MissNow.ToString ();
		}

		if (DisplayDone) {
			if (Timer > 1)
				Timer = 0.5f;
			else if (Timer >= 0)
				Timer -= Time.deltaTime;

			Leap.Frame frame = leap.Frame ();
			foreach (Leap.Gesture gesture in frame.Gestures()) {
				if (gesture.Type == Leap.Gesture.GestureType.TYPE_SWIPE) {
					Leap.SwipeGesture swipeGesture = new Leap.SwipeGesture (gesture);
					Leap.Vector gestureDirection = swipeGesture.Direction;
					//float x = gestureDirection.x;
					float y = gestureDirection.y;
					Debug.Log (y);
					if (y > 0.7)
						Retry ();
					if (y < -0.7)
						Back ();
				}
			}
		}

		if (DisplayDone && Timer < 0 && CoverColor.a > 0) {
			CoverColor.a -= Time.deltaTime * 2;
			SmallCover.color = CoverColor;
			if (CoverColor.a <= 0)
				Destroy (SmallCover);
		}
	}

	void CounterPlus (ref int Counter, int max, int step)
	{
		if (Counter < max) {
			if (max - Counter >= step)
				Counter += (max - Counter) / step;
			else
				Counter++;
			DisplayDone = false;
		}
	}

	void CounterPlus (ref long Counter, long max, int step)
	{
		if (Counter < max) {
			if (max - Counter >= step)
				Counter += (max - Counter) / step;
			else
				Counter++;
			DisplayDone = false;
		}
	}

	void Retry ()
	{
		Retrying = true;
	}

	void Back ()
	{
		Backing = true;
	}
}
