using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Leap;
using SimpleJSON;

public class MenuController : MonoBehaviour
{
	public GameObject menuitemobj;
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
	protected bool covermotion= true;
	protected float alphadirection = -1.0f;
	protected string playingsong ;
	protected List<string> diffcollection;
	protected Dictionary<string,List<string>> difflist;
	protected double changedifftime =1.5;
	protected Dictionary<string,float> backward;
	protected string difftonext;

	GUIText difftextobj;

//record the back needed
	// Use this for initialization
	void Start ()
	{
		difflist= new Dictionary<string, List<string>>();
		CoverTexture =  GameObject.Find ("Cover").GetComponent<GUITexture> ();
		// init 
		this.diff = new Dictionary<string, int> (){{"Easy",0},{"Normal",1},{"Hard",2}}; 
		this.song = new Dictionary<string,string> ();
		this.songname = new Dictionary<string, string>();
		this.songstart = new Dictionary<string, float>();
		this.backward = new Dictionary<string, float>();
		leap = new Controller ();
		diffcollection = new List<string> (){"Easy","Normal","Hard"};
		diffcolormap = new Dictionary<string, Color> {	{"Hard",new Color(249.0f/255,90.0f/255,101.0f/255,1.0f)},
														{"Easy",new Color(191.0f/255,255.0f/255,160.0f/255,1.0f)},
														{"Normal",new Color(58.0f/255,183.0f/255,239.0f/255,1.0f)}};
		// read dir from resource music
		//string[] files = Directory.GetDirectories (@"Assets/Resources/Music/");
		string[] files = (Resources.Load("Music/MusicList") as TextAsset).text.Replace("\r\n", "\n").Replace("\r","\n").Split("\n"[0]);
		Debug.Log(files.Length.ToString() + " music in list");
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

			difflist[Beatmap ["Title"]]= new List<string>();
			foreach (string difficulty in diffcollection) {
				if (Beatmap ["Difficulty"] [difficulty] ["Enable"].AsBool) {
					difflist[Beatmap ["Title"]].Add(difficulty);

				}
			}
			Vector3 pos = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
			this.backward[Beatmap ["Title"]] = (this.musicnum * 0.5f);
			if(PlayerPrefs.HasKey("BACKWARD")){
				Debug.Log ("has backword " +PlayerPrefs.GetFloat("BACKWARD"));
				pos.x-=PlayerPrefs.GetFloat("BACKWARD");
			}

			pos.x += (this.musicnum * 0.5f);
			GameObject menuitem_tmp = (GameObject)Instantiate (menuitemobj, pos, transform.rotation);
			Debug.Log ("loading " + "Music/" + folder + Beatmap ["Album"] ["Name"]);
			menuitem_tmp.GetComponent<GUITexture> ().texture = (Texture)Resources.Load ("Music/" + folder + "/" + Beatmap ["Album"] ["Name"]);
			//Debug.Log (menuitem_tmp.GetComponent<GUITexture> ().border.ToString ());
			menuitem_tmp.GetComponent<GUIText> ().text = Beatmap ["Title"];// +'\n'+ difficulty;
			Vector2 tpos = new Vector2 (0, - UnityEngine.Screen.height / 3.5f);
			menuitem_tmp.GetComponent<GUIText> ().pixelOffset = tpos;
			menuitem_tmp.GetComponent<GUIText> ().fontSize = (int)((tpos.y) / -83.5f * 15.0f);

			// solve the child GUIText OBJ

			menuitem_tmp.transform.GetChild(0).GetComponent<GUIText>().text = difflist[Beatmap ["Title"]][0];
			menuitem_tmp.transform.GetChild(0).GetComponent<GUIText>().color = diffcolormap[difflist[Beatmap ["Title"]][0]];
			tpos.y+= ((tpos.y) / -83.5f );
			menuitem_tmp.transform.GetChild(0).GetComponent<GUIText>().pixelOffset = tpos;
			menuitem_tmp.transform.GetChild(0).GetComponent<GUIText>().fontSize = (int)((tpos.y) / -83.5f * 15.0f);

			menuitem_tmp.transform.GetChild(1).GetComponent<GUIText>().text = Beatmap ["Artist"];
			menuitem_tmp.transform.GetChild(1).GetComponent<GUIText>().fontSize = (int)((tpos.y) / -83.5f * 10.0f);
			tpos.y-= ((tpos.y) / -83.5f * 23.0f);
			menuitem_tmp.transform.GetChild(1).GetComponent<GUIText>().pixelOffset = tpos;

			// save the Songs identity
			menulist.Add (menuitem_tmp);

			song [Beatmap ["Title"]] = folder;
			this.songname[Beatmap ["Title"]] = Beatmap ["Audio"] ["Name"];
			songstart[Beatmap ["Title"]] = float.Parse( Beatmap["PreviewTime"] );
			Debug.Log("music start point "+Beatmap ["Title"] + Beatmap["PreviewTime"]);
			this.musicnum ++;
		
			Debug.Log("location " + pos.x);
			if (pos.x < 0.6 && pos.x > 0.4){
				// play this item music
				string songpath = this.song [Beatmap ["Title"] ];
				string songname = this.songname[Beatmap ["Title"]];
				var music = GetComponent<AudioSource> ();
				music.clip = Resources.Load ("Music/" + songpath + "/" + songname) as AudioClip;
				Debug.Log("paly music demo "+"/Music/" + songpath + "/" + songname);
				music.time = this.songstart[Beatmap ["Title"]];
				music.loop = true;
				music.Play();

				playingsong = Beatmap ["Title"];
				//
				if(PlayerPrefs.HasKey("Difficulty")){
					Debug.Log("Has Selected Difficulty!");
					menuitem_tmp.transform.GetChild(0).GetComponent<GUIText>().text = diffcollection[PlayerPrefs.GetInt("Difficulty")];
					menuitem_tmp.transform.GetChild(0).GetComponent<GUIText>().color = diffcolormap[diffcollection[PlayerPrefs.GetInt("Difficulty")]];
				}
			}

		}
		//open the guesture
		leap.EnableGesture (Gesture.GestureType.TYPE_SWIPE);
		//leap.Config.SetFloat("Gesture.Swipe.MinLength", 100.0f);
		leap.Config.SetFloat ("Gesture.Swipe.MinVelocity", 750f);
		leap.Config.Save ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		//the cover	
		if(covermotion){
			Color tcolor = CoverTexture.color;
			float a =tcolor.a+Time.deltaTime*2*alphadirection;
			if (a<0.0f) {
				tcolor.a=0.0f;
				covermotion = false;
				alphadirection = 1.0f;
			}
			tcolor.a = a;
			CoverTexture.color = tcolor;
			Debug.Log(CoverTexture.color.ToString());
		}

		// guesture judgement
		if (motionlock) {
			if(lastmotion == 2 ){
				// change diff

				var color = difftextobj.color;
				if(changedifftime>1.2f){
					color.a -= (float)(Time.deltaTime/0.3);
				}else if(changedifftime>0.9f){
					color.a += (float)(Time.deltaTime/0.3);
				}
				difftextobj.color = color;
				//change when half time
				if((changedifftime-Time.deltaTime-1.2)*(changedifftime-1.2)<0){
					difftextobj.text = difftonext;
					var clr =  diffcolormap[difftonext];
					clr.a = 0f;
					difftextobj.color = clr;
				}
				//
				changedifftime-=Time.deltaTime;
				if(changedifftime<0){
					changedifftime=1.5;
					motionlock = false;
				}

			}else if (lastmotion != 0) {

				foreach (var item in menulist) {
					// move items and test move finished
					Vector3 position = item.transform.position;
					double lx = position.x;
					position.x += 0.5f * lastmotion * Time.deltaTime;
					item.transform.position = position;
					// if any item cross the 0.5f , release the lock;
					if ((position.x - 0.5) * (lx - 0.5) < 0){
						if(playingsong != item.GetComponent<GUIText> ().text){
							// play this item music
							string folder = this.song [item.GetComponent<GUIText> ().text];
							string name = this.songname[item.GetComponent<GUIText> ().text];
							var music = GetComponent<AudioSource> ();
							music.clip = Resources.Load ("Music/" + folder + "/" + name) as AudioClip;
							Debug.Log("paly music demo "+"/Music/" + folder + "/" + name);
							music.time = this.songstart[item.GetComponent<GUIText> ().text];
							music.loop = true;
							music.Play();
							playingsong = item.GetComponent<GUIText> ().text;
						}
						// unlock 
						this.motionlock = false;
					}
				}
				GetComponent<AudioSource> ().volume-=Time.deltaTime*1.2f;
			} else {
				foreach (var item in menulist) {
					Vector3 position = item.transform.position;
					if (position.x > 0.4 && position.x < 0.6) {

						// select movement

						position.y += 0.6f * Time.deltaTime;
						item.transform.position = position;
					}
					if (item.transform.position.y > 0.8) {  

						// enter the InGame Scene

						// select success
						Debug.Log ("enter " + this.song [item.GetComponent<GUIText> ().text ]);
						PlayerPrefs.SetString ("song", this.song [item.GetComponent<GUIText> ().text]);
						//enableSE = true;
						//enableBG = true;
						PlayerPrefs.SetInt ("enableSE", 1);
						PlayerPrefs.SetInt ("enableBG", 1);
						PlayerPrefs.SetFloat("BACKWARD",this.backward[item.GetComponent<GUIText> ().text]);
						Debug.Log ("set difficulty " + this.diff [ item.transform.GetChild(0).GetComponent<GUIText>().text]);
						PlayerPrefs.SetInt ("Difficulty", this.diff [item.transform.GetChild(0).GetComponent<GUIText>().text]);
						Application.LoadLevel ("InGame");
					}

				}
			}

		} else {
			// fix the low sound
			if(GetComponent<AudioSource> ().volume<1.0){
				GetComponent<AudioSource> ().volume+=Time.deltaTime*0.5f;
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
					if (x < - 0.7f && (menulist [0].transform.position.x + (menulist.Count - 1) * 0.5) > 0.6f) {
						lastmotion = -1;
					} else if (x > 0.7f && menulist [0].transform.position.x < 0.4f) {
						lastmotion = 1;
					}  else if (y < -0.7) { 
						// change difficult
						motionlock = true;
						foreach (var item in menulist) {
							Vector3 position = item.transform.position;
							if (position.x > 0.4 && position.x < 0.6) {
								var list = difflist[item.GetComponent<GUIText> ().text];
								int index = list.IndexOf(item.transform.GetChild(0).GetComponent<GUIText>().text);
								Debug.Log("diff content list "+ list.ToString()+ " index: "+index);
								
								index = (index+1)%list.Count;
								difftonext = list[index];
								difftextobj =item.transform.GetChild(0).GetComponent<GUIText>();
								//item.transform.GetChild(0).GetComponent<GUIText>().text = list[index];
								//item.transform.GetChild(0).GetComponent<GUIText>().color = diffcolormap[list[index]];

							}
						}
						lastmotion = 2;

					}else if (y > 0.6) {
						Debug.Log("select music guesture");
						lastmotion = 0;
						covermotion = true;
					}else {
						return;
					}
					motionlock = true;
				}
			}
		}
	}
}
