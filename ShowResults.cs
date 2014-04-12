using UnityEngine;
using System.Collections;

public class ShowResults : MonoBehaviour {
	
	void OnGUI()
	{
		if (GUI.Button (new Rect(280, 300, 250, 100), "Back To Title")) 
			ReturnToTitle ();
	}

	// Use this for initialization
	void Start () 
	{
		Network.Disconnect();
	}

	public void ReturnToTitle()
	{
		Application.LoadLevel (0);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
