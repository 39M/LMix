using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
public class menucontroller : MonoBehaviour {
	public GameObject menuitemobj;
	public List<GameObject> menulist= new List<GameObject> ();
	// Use this for initialization
	void Start () {
		List<string> a = new List<string>() {"1","2"};
		var i = 0;
		foreach(var item in a){
			Vector3 pos=new Vector3(transform.position.x,transform.position.y,transform.position.z);
			pos.x+=i;
			GameObject menuitem_tmp = (GameObject) Instantiate(menuitemobj,pos,transform.rotation);
			menuitem_tmp.GetComponent<GUITexture>().texture=(Texture)Resources.Load(item);//AssetBundle.CreateFromFile("tmpfile/"+item+".jpg");
			menuitem_tmp.GetComponent<GUIText>().text=item;
			menulist.Add(menuitem_tmp);
			i++;
		}

	}
	
	// Update is called once per frame
	void Update () {
		foreach (var item in menulist) {
			Vector3 pos=item.transform.position;
			pos.x-=0.005f;
			item.transform.position=pos;
		}
	}
}
