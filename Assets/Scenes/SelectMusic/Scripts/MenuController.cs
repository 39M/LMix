using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Leap;
using SimpleJSON;
public class MenuController : MonoBehaviour {
	public GameObject menuitemobj;
	public List<GameObject> menulist= new List<GameObject> ();
	public int musicnum = 0;
	protected bool motionlock=false;
	protected bool selectlock = false;
	protected double lastx;
	protected Controller leap;
	protected Vector3 lastpos;
	protected Dictionary<string,int> diff ;
	protected Dictionary<string,string> song ;
	// Use this for initialization
	void Start () {
		// init 
		this.diff = new Dictionary<string, int>();
		this.song = new Dictionary<string,string>();
		leap = new Controller ();

		// read dir from resource music
		string[] files = Directory.GetDirectories (@"Assets/Resources/Music/");
		List<string> a = new List<string> ();
	
		foreach (var item in files) {
			string folder = Path.GetFileName(item);
			Debug.Log(" Read Music Dir " + folder + " and load beatmap.");

			TextAsset f = Resources.Load ("Music/" + folder + "/beatmap") as TextAsset;
			if (f == null) 	{
				Debug.Log ("Beatmap for "+ folder+" load failed !!!");
				return;
			}
			Debug.Log("Beatmap for "+ folder+"load success");
			JSONNode Beatmap = JSON.Parse(f.ToString());

			List<string> diffcollection = new List<string>(){"Easy","Normal","Hard"};
			Dictionary<string,int> diffmap = new Dictionary<string, int>(){{"Easy",0},{"Normal",1},{"Hard",2}}; 
			foreach(string difficulty in diffcollection){
				if(Beatmap["Difficulty"][difficulty]["Enable"].AsBool){
					Vector3 pos=new Vector3(transform.position.x,transform.position.y,transform.position.z);
					pos.x += (this.musicnum*0.5f);
					GameObject menuitem_tmp = (GameObject) Instantiate(menuitemobj,pos,transform.rotation);
					Debug.Log("loading "+"Music/"+folder+Beatmap["Album"]["Name"]);
					menuitem_tmp.GetComponent<GUITexture>().texture=(Texture)Resources.Load("Music/"+folder+"/"+Beatmap["Album"]["Name"]);
					menuitem_tmp.GetComponent<GUIText>().text=Beatmap["Title"]+difficulty;
					menulist.Add(menuitem_tmp);
					diff[Beatmap["Title"]+difficulty]=diffmap[difficulty];
					song[Beatmap["Title"]+difficulty]=folder;
					this.musicnum ++ ;
				}
			}
		}
		//
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!motionlock) {
			Frame fream = leap.Frame ();
			Hand hand = fream.Hands [0];
			lastpos = new Vector3( hand.WristPosition.x/100.0f, hand.WristPosition.y/100.0f-0.7f, 5) ;
		}
		if (lastpos.x < 0.3f || lastpos.x > 0.7f) {
			// the first and last song can't out off the edge
			if(!selectlock){
				if (menulist [0].transform.position.x > 0.5f && lastpos.x > 0.5) {
					motionlock = false;
					Debug.Log ("z");
					lastpos.x = 0.5f;
					return;
				} else if ((menulist [0].transform.position.x + (menulist.Count - 1) * 0.5) < 0.5f && lastpos.x < 0.5) {
					motionlock = false;
					Debug.Log ("y");
					lastpos.x = 0.5f;
					return;
				}
				
				motionlock = true;
				foreach (var item in menulist) {
					Vector3 position = item.transform.position;
					double lx = position.x;
					position.x += (lastpos.x - 0.5f) * Time.deltaTime;
					item.transform.position = position;
					// if any item cross the 0.5f , release the lock;
					if ((position.x - 0.5) * (lx - 0.5) < 0)
						motionlock = false;
				}
			}
		} else {
			if (!motionlock) {
				Frame fream = leap.Frame ();
				Hand hand = fream.Hands [0];
				if(hand.WristPosition.y/100.0f>1.5){
					foreach (var item in menulist) {
						Vector3 position=item.transform.position;
						if(position.x>0.4 && position.x<0.6){
							position.y+=0.3f*Time.deltaTime;
							item.transform.position=position;
						}
						if(item.transform.position.y>0.8){
							// select success
							Debug.Log("enter "+this.song[item.GetComponent<GUIText>().text]);
							PlayerPrefs.SetString("song",this.song[item.GetComponent<GUIText>().text]);
							//enableSE = true;
							//enableBG = true;
							PlayerPrefs.SetInt("enableSE",1);
							PlayerPrefs.SetInt("enableBG",1);
							Debug.Log("set difficulty "+ this.diff[item.GetComponent<GUIText>().text]);
							PlayerPrefs.SetInt("Difficulty",this.diff[item.GetComponent<GUIText>().text]);
							Application.LoadLevel("InGame");
						}
					}
				}else{
					foreach (var item in menulist) {
						Vector3 position=item.transform.position;
						if(position.x>0.4 && position.x<0.6){
							position.y=0.5f;
							item.transform.position=position;
						}
					}
				}
			}
		}


	}
}
