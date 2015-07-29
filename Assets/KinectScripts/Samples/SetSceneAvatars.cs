using UnityEngine;
using System.Collections;

public class SetSceneAvatars : MonoBehaviour 
{

	void Start () 
	{
		KinectManager manager = KinectManager.Instance;
		
		if(manager)
		{
			manager.ClearKinectUsers();
			manager.Player1Avatars.Clear();
			
			AvatarController[] avatars = FindObjectsOfType(typeof(AvatarController)) as AvatarController[];
			
			foreach(AvatarController avatar in avatars)
			{
				manager.Player1Avatars.Add(avatar.gameObject);
			}
			
			manager.ResetAvatarControllers();

			// add available gesture listeners
			manager.gestureListeners.Clear();

			MonoBehaviour[] listeners = FindObjectsOfType(typeof(MonoBehaviour)) as MonoBehaviour[];

			foreach(MonoBehaviour listener in listeners)
			{
				if(typeof(KinectGestures.GestureListenerInterface).IsAssignableFrom(listener.GetType()))
				{
					KinectGestures.GestureListenerInterface gl = (KinectGestures.GestureListenerInterface)listener;
					manager.gestureListeners.Add(gl);
				}
			}

		}
	}
	
}
