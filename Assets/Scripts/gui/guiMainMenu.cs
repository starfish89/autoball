using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class guiMainMenu : MonoBehaviour {

	public GUISkin menuSkin ;
	public GUISkin largeFont;
    private multiPlayerScript multiPlayer;

	private enum states : int { main, host, join, gameLobby};
	private int state = 0;

	private Rect rectMainMenu;
	private Rect rectJoinMenu;
	private Rect rectHostMenu;
	private Rect rectGameLobby;

    private string playerName = "player";
	private string hostGameName = "gamename";
	private string connectIp = "127.0.0.1";
	private int hostPort = 6666;
	private int playerCount = 8;

	private int windowWidth = 600;
	private int windowHeight = 300;
	private int windowX;
	private int windowY;

	private Vector2 scrollPosition;

    /// <summary>
    /// Wird beim ersten Start aufgerufen
    /// </summary>
    public void Start(){
        
        // vorherigen Player Namen holen
        playerName = PlayerPrefs.GetString("playerName");
        
        // Multiplayer Script laden
        multiPlayer = GetComponent("multiPlayerScript") as multiPlayerScript;


		windowX = ( Screen.width - windowWidth ) / 2;
		windowY = ( Screen.height - windowHeight ) / 2;
    }

	/// <summary>
	/// Raises the GUI event.
	/// </summary>




	public void OnGUI(){

        if (Network.isClient || Network.isServer)
        {
			state = (int)states.gameLobby;
		}



		//Apply the skin
		GUI.skin = menuSkin;

		rectMainMenu = new Rect( windowX, windowY, windowWidth, windowHeight );
		rectJoinMenu = new Rect( windowX, windowY, windowWidth, windowHeight );
		rectHostMenu = new Rect( windowX, windowY, windowWidth, windowHeight );
		rectGameLobby = new Rect(windowX, windowY, windowWidth, windowHeight);


		//Welches Menu?
		switch(state){
		case (int)states.main:
			GUILayout.Window(0, rectMainMenu, drawMainMenu, "Main Menu" );	
			break;
		case (int)states.host:
			GUILayout.Window(1, rectHostMenu, drawHostMenu, "Host a Game" );
			break;
		case (int)states.join:
			GUILayout.Window(2, rectJoinMenu, drawJoinMenu, "Find a Game" );
			break;
		case (int)states.gameLobby:
			GUILayout.Window(3, rectGameLobby, drawGameLobby, hostGameName);
			break;
		}

		GUI.skin = largeFont;

		GUI.Label(new Rect ((Screen.width)/2 -(Screen.width)/4,0,(Screen.width)/2,(Screen.height)/4), "Autoball");
	}
	/// <summary>
	/// Draws the main menu.
	/// </summary>
	/// <param name="windowID">Window I.</param>

	private void drawMainMenu(int windowID){

		GUILayout.BeginVertical();
		if(GUILayout.Button("Host Game", GUILayout.ExpandHeight(true))){
			state = (int)states.host;
		}

		if(GUILayout.Button("Join Game", GUILayout.ExpandHeight(true))){
			multiPlayer.fetchHostList();
			state = (int)states.join;
		}

		if(GUILayout.Button ("Quit", GUILayout.ExpandHeight(true))){
			Application.Quit();
		}
		GUILayout.EndVertical();
	}

	/// <summary>
	/// Draws the host menu.
	/// </summary>
	/// <param name="windowID">Window I.</param>

	private void drawHostMenu(int windowID)
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("Player Name:", GUILayout.Width(150));
		playerName = GUILayout.TextField(playerName);
		GUILayout.EndHorizontal();

		if(GUI.changed){
			PlayerPrefs.SetString("playerName", playerName);
		}
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Game Name:", GUILayout.Width(150));
		hostGameName = GUILayout.TextField(hostGameName);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Players:", GUILayout.Width(150));
		playerCount = Convert.ToInt16(GUILayout.TextField(playerCount.ToString()));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Map:", GUILayout.Width(150));

		GUILayout.EndHorizontal();

		GUILayout.Space(50);
		//GUILayout.FlexibleSpace();

		if(GUILayout.Button("Host", GUILayout.ExpandHeight(true))){
			multiPlayer.startHost(playerCount, hostPort, playerName);
		}
		if(GUILayout.Button("Back", GUILayout.ExpandHeight(true))){
			state = (int)states.main;
		}
	}
	/// <summary>
	/// Draws the join menu.
	/// </summary>
	/// <param name="windowID">Window I.</param>

	private void drawJoinMenu(int windowID)
	{

		GUILayout.BeginVertical();

		GUILayout.BeginHorizontal();
			
		GUILayout.Label("Player Name:", GUILayout.Width(150));
		playerName =  GUILayout.TextField(playerName);

		if(GUI.changed){
			PlayerPrefs.SetString("playerName", playerName);
		}

		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();

		GUILayout.Label("Direct Connection:", GUILayout.Width(150));
		connectIp = GUILayout.TextField(connectIp);
		if(GUILayout.Button("Connect")){
			multiPlayer.connect(connectIp, playerName);
		}

		GUILayout.EndHorizontal();

		//Host List
		scrollPosition = GUILayout.BeginScrollView (scrollPosition);

		if(multiPlayer.hostData.Length>=1 && multiPlayer.hostData != null){
		foreach (HostData element in multiPlayer.hostData)
			{
				if(element!=null){
					GUILayout.BeginHorizontal();

					// Do not display NAT enabled games if we cannot do NAT punchthrough
						var name = element.gameName + " ";
						GUILayout.Label(name);	
						GUILayout.FlexibleSpace();
						GUILayout.Label(element.connectedPlayers + "/" + element.playerLimit);

						GUILayout.FlexibleSpace();
						GUILayout.Label("[" + element.ip[0] + ":" + element.port + "]");	
						GUILayout.FlexibleSpace();
						
						if(!multiPlayer.nowConnecting){
							if (GUILayout.Button("Connect"))
							{
								multiPlayer.connect(element.ip, playerName);
							}
						}else{
							GUILayout.Button("Connecting");
						}
						GUILayout.Space(15);

					GUILayout.EndHorizontal();
				}
			}
		}		
		GUILayout.EndScrollView ();


		GUILayout.Space(50);

		if (GUILayout.Button ("Refresh", GUILayout.ExpandHeight(true))){
			multiPlayer.requestHost();	
		}

		if (GUILayout.Button("Back", GUILayout.ExpandHeight(true))){
        	state = (int)states.main;
        }

		GUILayout.EndVertical();

	}


	private void drawGameLobby(int windowID){

		GUILayout.BeginVertical();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Game Name:", GUILayout.Width(150));
		GUILayout.EndHorizontal();

		scrollPosition = GUILayout.BeginScrollView (scrollPosition);

		GUI.Box (new Rect(50,20,500,200), "Players");

		if(multiPlayer.playerList.Count>=1){

			GUILayout.Space (50);
			foreach (PlayerNode element in multiPlayer.playerList)
			{
				
				GUILayout.BeginHorizontal();
				GUILayout.Space(100);
				GUILayout.Label(element.playerName);
				GUILayout.EndHorizontal();	
			}
			
			
		}		
		GUILayout.EndScrollView ();

		GUILayout.FlexibleSpace();

		if(Network.isServer){
			if(GUILayout.Button("Start", GUILayout.ExpandHeight(true))){
				multiPlayer.loadLevel();
			}

			if(GUILayout.Button("Back", GUILayout.ExpandHeight(true))){
			if(Network.isClient){
				multiPlayer.disconnect();
			} else {
				multiPlayer.disconnectServer();
			}
			state = (int)states.main;
		}

	
		}

		GUILayout.EndVertical();
	}
}