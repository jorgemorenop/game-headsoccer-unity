using UnityEngine;
using System.Collections;

public class checkMenu : MonoBehaviour {

	public GUISkin theSkin;

	void OnGUI() {
		GUI.skin = theSkin;

		if (GUI.Button (new Rect (Screen.width/2 - 50, 70, 140, 80), "START") )
		{
			GetComponent<AudioSource>().Play ();
			Application.LoadLevel(1);
		} 
	}
}
