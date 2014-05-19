using UnityEngine;
using System.Collections;

public class gameMenu : MonoBehaviour {

	private int windowWidth = 600;
	private int windowHeight = 300;
	private int windowX;
	private int windowY;
	private bool guiOn = false;

	private Rect rectMainMenu;

	// Use this for initialization
	void Start () {
	
		windowX = ( Screen.width - windowWidth ) / 2;
		windowY = ( Screen.height - windowHeight ) / 2;

	}

	void Update(){

		if(Input.GetKeyUp(KeyCode.Escape)){
			
			if(guiOn){
				guiOn = false;
			} else {
				guiOn = true;
			}
		}


	}

	// Update is called once per frame
	void OnGUI () {

		if(guiOn){
			rectMainMenu = new Rect( windowX, windowY, windowWidth, windowHeight );
			GUILayout.Window(0, rectMainMenu, drawMainMenu, "Main Menu" );
		}
	}

	public void drawMainMenu(int windowd){

		GUILayout.BeginVertical();

		if(GUILayout.Button("Back")){
			guiOn = false;
		}

		if(GUILayout.Button("Exit Game")){
			Application.LoadLevel("MainMenu");
			Network.Disconnect();
		}

		GUILayout.EndVertical();

	}
}