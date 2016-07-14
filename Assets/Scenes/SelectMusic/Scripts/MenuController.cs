using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Leap;
using SimpleJSON;

public class MenuController : MonoBehaviour
{
	public GameObject menuitemobj;
    public GameObject canvas;
	public List<GameObject> menulist = new List<GameObject> ();
	public int musicnum = 0;
	protected bool motionlock = false;
	protected double lastx;
	protected Controller leap;
	protected int lastmotion;
	protected Dictionary<string,int> diff ;
	protected Dictionary<string,string> song ;
	protected Dictionary<string,string> songname ;
	protected Dictionary<string,float> songstart ;
	protected Dictionary<string,Color> diffcolormap ;
	protected GUITexture CoverTexture;
	protected bool covermotion = true;
	protected float alphadirection = -1.0f;
	protected string playingsong ;
	protected List<string> diffcollection;
	protected Dictionary<string,List<string>> difflist;
	protected double changedifftime = 1.5;
	protected Dictionary<string,float> backward;
	protected string difftonext;
	protected bool changesong = true;

	protected float changedifftimeendtime =0;

	protected bool QuitGameFlag =false;

    float spaceHorizontal = 5.63f;
    int curID;

    Text difftextobj;

//record the back needed
	// Use this for initialization
	void Start ()
	{
        //PlayerPrefs.DeleteAll();
		difflist = new Dictionary<string, List<string>> ();
		CoverTexture = GameObject.Find ("Cover").GetComponent<GUITexture> ();
		// init 
		this.diff = new Dictionary<string, int> (){{"Easy",0},{"Normal",1},{"Hard",2}}; 
		this.song = new Dictionary<string,string> ();
		this.songname = new Dictionary<string, string> ();
		this.songstart = new Dictionary<string, float> ();
		this.backward = new Dictionary<string, float> ();
		leap = new Controller ();
		diffcollection = new List<string> (){"Easy","Normal","Hard"};
		diffcolormap = new Dictionary<string, Color> {	{"Hard",new Color(249.0f/255,90.0f/255,101.0f/255,1.0f)},
														{"Easy",new Color(191.0f/255,255.0f/255,160.0f/255,1.0f)},
														{"Normal",new Color(58.0f/255,183.0f/255,239.0f/255,1.0f)}};
		// read dir from resource music
		//string[] files = Directory.GetDirectories (@"Assets/Resources/Music/");
		string[] files = (Resources.Load ("Music/MusicList") as TextAsset).text.Replace ("\r\n", "\n").Replace ("\r", "\n").Split ("\n" [0]);
		Debug.Log (files.Length.ToString () + " music in list");
		//List<string> a = new List<string> ();
	
		foreach (var item in files) {
			//string folder = Path.GetFileName (item);
			string folder = item;
			//Debug.Log(item.Length);
			//Debug.Log(folder);
			Debug.Log (" Read Music Dir " + folder + " and load beatmap.");

			TextAsset f = Resources.Load ("Music/" + folder + "/beatmap") as TextAsset;
			if (f == null) {
				Debug.Log ("Beatmap for " + folder + " load failed !!!");
				return;
			}
			Debug.Log ("Beatmap for " + folder + "load success");
			JSONNode Beatmap = JSON.Parse (f.ToString ());

			difflist [Beatmap ["Title"]] = new List<string> ();
			foreach (string difficulty in diffcollection) {
				if (Beatmap ["Difficulty"] [difficulty] ["Enable"].AsBool) {
					difflist [Beatmap ["Title"]].Add (difficulty);
				}
			}
            //Vector3 pos = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
            Vector3 pos = new Vector3(0, 0, canvas.transform.position.z);
            this.backward [Beatmap ["Title"]] = (this.musicnum * spaceHorizontal);
			if (PlayerPrefs.HasKey ("BACKWARD")) {
				Debug.Log ("has backword " + PlayerPrefs.GetFloat ("BACKWARD"));
				pos.x -= PlayerPrefs.GetFloat ("BACKWARD");
			}

			pos.x += (this.musicnum * spaceHorizontal);
            //GameObject menuitem_tmp = (GameObject)Instantiate (menuitemobj, pos, transform.rotation);
            //Debug.Log ("loading " + "Music/" + folder + Beatmap ["Album"] ["Name"]);
            //menuitem_tmp.GetComponent<GUITexture> ().texture = (Texture)Resources.Load ("Music/" + folder + "/" + Beatmap ["Album"] ["Name"]);
            ////Debug.Log (menuitem_tmp.GetComponent<GUITexture> ().border.ToString ());
            //menuitem_tmp.GetComponent<GUIText> ().text = Beatmap ["Title"];// +'\n'+ difficulty;
            //Vector2 tpos = new Vector2 (0, - UnityEngine.Screen.height / 3.5f);
            //menuitem_tmp.GetComponent<GUIText> ().pixelOffset = tpos;
            //menuitem_tmp.GetComponent<GUIText> ().fontSize = (int)((tpos.y) / -83.5f * 15.0f);

            //// solve the child GUIText OBJ

            //menuitem_tmp.transform.GetChild (0).GetComponent<GUIText> ().text = difflist [Beatmap ["Title"]] [0];
            //menuitem_tmp.transform.GetChild (0).GetComponent<GUIText> ().color = diffcolormap [difflist [Beatmap ["Title"]] [0]];
            //tpos.y += ((tpos.y) / -83.5f);
            //menuitem_tmp.transform.GetChild (0).GetComponent<GUIText> ().pixelOffset = tpos;
            //menuitem_tmp.transform.GetChild (0).GetComponent<GUIText> ().fontSize = (int)((tpos.y) / -83.5f * 15.0f);

            //menuitem_tmp.transform.GetChild (1).GetComponent<GUIText> ().text = Beatmap ["Artist"];
            //menuitem_tmp.transform.GetChild (1).GetComponent<GUIText> ().fontSize = (int)((tpos.y) / -83.5f * 10.0f);
            //tpos.y -= ((tpos.y) / -83.5f * 20.0f);
            //menuitem_tmp.transform.GetChild (1).GetComponent<GUIText> ().pixelOffset = tpos;

            GameObject menuitem_tmp = (GameObject)Instantiate(menuitemobj, pos, transform.rotation);
            Debug.Log(pos);
            Debug.Log("loading " + "Music/" + folder + Beatmap["Album"]["Name"]);
            menuitem_tmp.transform.SetParent(canvas.transform);
            menuitem_tmp.GetComponent<UnityEngine.UI.Image>().sprite = Resources.Load<Sprite>("Music/" + folder + "/" + Beatmap["Album"]["Name"]);
            menuitem_tmp.transform.GetChild(0).GetComponent<Text>().text = Beatmap["Title"];
            menuitem_tmp.transform.GetChild(1).GetComponent<Text>().text = difflist[Beatmap["Title"]][0];
            menuitem_tmp.transform.GetChild(1).GetComponent<Text>().color = diffcolormap[difflist[Beatmap["Title"]][0]];
            menuitem_tmp.transform.GetChild(2).GetComponent<Text>().text = Beatmap["Artist"];



            // save the Songs identity
            menulist.Add (menuitem_tmp);

			song [Beatmap ["Title"]] = folder;
			this.songname [Beatmap ["Title"]] = Beatmap ["Audio"] ["Name"];
			songstart [Beatmap ["Title"]] = float.Parse (Beatmap ["PreviewTime"]);
			Debug.Log ("music start point " + Beatmap ["Title"] + Beatmap ["PreviewTime"]);
			this.musicnum ++;
		
			Debug.Log ("location " + pos.x);
			if (pos.x < spaceHorizontal * 0.5f && pos.x > -spaceHorizontal * 0.5f) {
				// play this item music
				string songpath = this.song [Beatmap ["Title"]];
				string songname = this.songname [Beatmap ["Title"]];
				var music = GetComponent<AudioSource> ();
				music.clip = Resources.Load ("Music/" + songpath + "/" + songname) as AudioClip;
				Debug.Log ("paly music demo " + "/Music/" + songpath + "/" + songname);
				music.time = this.songstart [Beatmap ["Title"]];
				music.loop = true;
				music.Play ();

				playingsong = Beatmap ["Title"];
				//
				if (PlayerPrefs.HasKey ("Difficulty")) {
					Debug.Log ("Has Selected Difficulty!");
					menuitem_tmp.transform.GetChild (1).GetComponent<Text> ().text = diffcollection [PlayerPrefs.GetInt ("Difficulty")];
					menuitem_tmp.transform.GetChild (1).GetComponent<Text> ().color = diffcolormap [diffcollection [PlayerPrefs.GetInt ("Difficulty")]];
				}
			}

		}
        curID = (int) Mathf.Abs(menulist[0].transform.position.x / spaceHorizontal);

		//open the guesture
		leap.EnableGesture (Gesture.GestureType.TYPE_SWIPE);
		//leap.Config.SetFloat("Gesture.Swipe.MinLength", 100.0f);
		leap.Config.SetFloat ("Gesture.Swipe.MinVelocity", 750f);
		leap.Config.Save ();
	}
	
	// Update is called once per frame
	void Update ()
	{
        Debug.Log("Last Motion = " + lastmotion);
        Debug.Log("Motion lock = " + motionlock);
        //the cover	
        if (covermotion) {
			Color tcolor = CoverTexture.color;
			float a = tcolor.a + Time.deltaTime * 2 * alphadirection;
			if (a < 0.0f) {

				tcolor.a = 0.0f;
				covermotion = false;
				alphadirection = 1.0f;

			}else{
				tcolor.a = a;
				CoverTexture.color = tcolor;
			}


			if(QuitGameFlag && a>0.8f ){
				
				QuitGameFlag = false;
				Debug.Log("system exit!");
				Application.Quit();
			}
			//Debug.Log (CoverTexture.color.ToString ());
		}
		//Debug.Log (covermotion.ToString()+alphadirection.ToString()+QuitGameFlag.ToString());
		// guesture judgement
		if (motionlock) {
			if (lastmotion == 2) {
				// change diff

				var color = difftextobj.color;
				if (changedifftime > 1.2f) {
					color.a -= (float)(Time.deltaTime / 0.3);
				} else if (changedifftime > 0.9f) {
					color.a += (float)(Time.deltaTime / 0.3);
				}
				difftextobj.color = color;
				//change when half time
				if ((changedifftime - Time.deltaTime - 1.2) * (changedifftime - 1.2) < 0) {
					difftextobj.text = difftonext;
					var clr = diffcolormap [difftonext];
					clr.a = 0f;
					difftextobj.color = clr;
				}
				//
				changedifftime -= Time.deltaTime;
				if (changedifftime < changedifftimeendtime) {
					changedifftime = 1.5;
					motionlock = false;
				}

			} else if (lastmotion == -1||lastmotion == 1) {

                int i = 0;
				foreach (var item in menulist) {
					// move items and test move finished
					Vector3 position = item.transform.position;
                    float lx = position.x;
                    //position.x += spaceHorizontal * lastmotion * Time.deltaTime;
                    float targetX;
                    if (lastmotion == 1)
                        targetX = (i - curID) * spaceHorizontal + spaceHorizontal;
                    else
                        targetX = (i - curID) * spaceHorizontal - spaceHorizontal;
                    position.x = Mathf.SmoothStep(position.x, targetX, 0.15f);
					item.transform.position = position;
                    // if any item cross the 0.5f , release the lock;
                    if ((position.x + 1e-2 * lastmotion) * (lx) < 0)
                    {
                        //if (Mathf.Abs(position.x - targetX) < 1e-2) {
                            if (playingsong != item.transform.GetChild(0).GetComponent<Text> ().text) {

                            // play this item music
                            int j = 0;
                            foreach (var in_item in menulist) {
								Vector3 in_postion = in_item.transform.position;
								in_postion.x = (j - curID) * spaceHorizontal + lastmotion * spaceHorizontal;
                                in_item.transform.position = in_postion;
                                j++;
							}

							string folder = this.song [item.transform.GetChild(0).GetComponent<Text> ().text];
							string name = this.songname [item.transform.GetChild(0).GetComponent<Text> ().text];
                            StartCoroutine(LoadMusic(item, folder, name));
							playingsong = item.transform.GetChild(0).GetComponent<Text> ().text;
						}
						// unlock 
						this.motionlock = false;
					}
                    i++;
				}
				GetComponent<AudioSource> ().volume -= Time.deltaTime * 1.2f;
			} else if (lastmotion == 0){
				foreach (var item in menulist) {
					Vector3 position = item.transform.position;
					if (position.x > -spaceHorizontal * 0.5f && position.x < spaceHorizontal * 0.5f) {

						// select movement

						//position.y += 2f * Time.deltaTime;
                        position.y = Mathf.SmoothStep(position.y, 1f, 0.1f);
						item.transform.position = position;
					}
					if (item.transform.position.y > 0.5f) {  

						// enter the InGame Scene

						// select success
						Debug.Log ("enter " + this.song [item.transform.GetChild(0).GetComponent<Text> ().text]);
						PlayerPrefs.SetString ("song", this.song [item.transform.GetChild(0).GetComponent<Text> ().text]);
						//enableSE = true;
						//enableBG = true;
						PlayerPrefs.SetInt ("enableSE", 1);
						PlayerPrefs.SetInt ("enableBG", 1);
						PlayerPrefs.SetFloat ("BACKWARD", this.backward [item.transform.GetChild(0).GetComponent<Text> ().text]);
						Debug.Log ("set difficulty " + this.diff [item.transform.GetChild (1).GetComponent<Text> ().text]);
						PlayerPrefs.SetInt ("Difficulty", this.diff [item.transform.GetChild (1).GetComponent<Text> ().text]);
						Application.LoadLevel ("InGame");
					}

				}
			}

		} else {
            curID = Mathf.RoundToInt(Mathf.Abs(menulist[0].transform.position.x / spaceHorizontal));
            Debug.Log("Cur ID = " + curID);

            // fix the low sound
            if (GetComponent<AudioSource> ().volume < 1.0) {
				GetComponent<AudioSource> ().volume += Time.deltaTime * 0.5f;
			}
			// keyboard
			if (!QuitGameFlag && Input.GetKey(KeyCode.Escape)){
				Debug.Log("get key escape");
				//Application.Quit();
				alphadirection = 1f;
				covermotion = true;
				QuitGameFlag = true;
				lastmotion = 3;
				motionlock = true;
			}
			if (Input.GetKey (KeyCode.A)||Input.GetKey (KeyCode.LeftArrow)) {
				if(menulist [0].transform.position.x < -spaceHorizontal * 0.5f){
					lastmotion = 1;
					motionlock = true;
				}
			}
			if (Input.GetKey (KeyCode.D)||Input.GetKey (KeyCode.RightArrow)) {
				if((menulist [0].transform.position.x + (menulist.Count - 1) * spaceHorizontal) > spaceHorizontal * 0.5f)
                {
					lastmotion = -1;
					motionlock = true;
				}
			}
			if (Input.GetKey (KeyCode.S)||Input.GetKey (KeyCode.DownArrow)|| Input.GetKey (KeyCode.Space)) {
				changedifftimeendtime = 0.8f;
				motionlock = true;
				foreach (var item in menulist) {
					Vector3 position = item.transform.position;
					if (position.x > -spaceHorizontal * 0.5f && position.x < spaceHorizontal * 0.5f) {
						var list = difflist [item.transform.GetChild(0).GetComponent<Text> ().text];
						int index = list.IndexOf (item.transform.GetChild (1).GetComponent<Text> ().text);
						Debug.Log ("diff content list " + list.ToString () + " index: " + index);
						
						index = (index + 1) % list.Count;
						difftonext = list [index];
						difftextobj = item.transform.GetChild (1).GetComponent<Text> ();
						//item.transform.GetChild(0).GetComponent<GUIText>().text = list[index];
						//item.transform.GetChild(0).GetComponent<GUIText>().color = diffcolormap[list[index]];
					}
				}
				lastmotion = 2;
			}
			if(Input.GetKey (KeyCode.W)||Input.GetKey (KeyCode.UpArrow)||Input.GetKey (KeyCode.Return)){
				covermotion = true;
				lastmotion = 0;
				motionlock = true;
			}
			//
			Frame sfream = leap.Frame ();
			foreach (var gesture in sfream.Gestures()) {
				if (gesture.Type == Gesture.GestureType.TYPE_SWIPE) {
					SwipeGesture swipeGesture = new SwipeGesture (gesture);
					Debug.Log (" gesture read success : " + swipeGesture.Direction.ToString ());
					Vector gestureVector = swipeGesture.Direction;
					float x = gestureVector.x;
					float y = gestureVector.y;
					if (x < - 0.7f && (menulist [0].transform.position.x + (menulist.Count - 1) * 0.5) > spaceHorizontal * 0.5f) {
						lastmotion = -1;
					} else if (x > 0.7f && menulist [0].transform.position.x < -spaceHorizontal * 0.5f) {
						lastmotion = 1;
					} else if (y < -0.7f) { 
						// change difficult
						changedifftimeendtime = 0.0f;
						motionlock = true;
						foreach (var item in menulist) {
							Vector3 position = item.transform.position;
							if (position.x > -spaceHorizontal * 0.5f && position.x < spaceHorizontal * 0.5f) {
								var list = difflist [item.transform.GetChild(0).GetComponent<Text> ().text];
								int index = list.IndexOf (item.transform.GetChild (1).GetComponent<Text> ().text);
								Debug.Log ("diff content list " + list.ToString () + " index: " + index);
								
								index = (index + 1) % list.Count;
								difftonext = list [index];
								difftextobj = item.transform.GetChild (1).GetComponent<Text> ();
								//item.transform.GetChild(0).GetComponent<GUIText>().text = list[index];
								//item.transform.GetChild(0).GetComponent<GUIText>().color = diffcolormap[list[index]];
							}
						}
						lastmotion = 2;

					} else if (y > 0.6) {
						Debug.Log ("select music guesture");
						lastmotion = 0;
						covermotion = true;
					} else {
						return;
					}
					motionlock = true;
				}
			}
		}
	}

    IEnumerator LoadMusic(GameObject item, string folder, string name)
    {
        var music = GetComponent<AudioSource>();
        ResourceRequest request = Resources.LoadAsync("Music/" + folder + "/" + name);
        yield return request;
        music.clip = request.asset as AudioClip;
        Debug.Log("paly music demo " + "/Music/" + folder + "/" + name);
        music.time = this.songstart[item.transform.GetChild(0).GetComponent<Text>().text];
        music.loop = true;
        music.Play();
    }
}
