using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class multiPlayerScript : MonoBehaviour {

	
	public string gameName = "AutoBall";
	public int serverPort =  6666;
	private string playerName = "";
	public HostData[] hostData = new HostData[0];

	public multiPlayerManager mpm;


	private int lastLevelPrefix = 0;
	public bool nowConnecting = false;

	public void Awake(){
		requestHost();
		mpm = (multiPlayerManager) GameObject.FindGameObjectWithTag("mp").GetComponent("multiPlayerManager");
	}

	public void requestHost(){
		MasterServer.ClearHostList();
		MasterServer.RequestHostList(gameName);
	}

	public void Update () {
		fetchHostList();
	}

	public void startHost(int players, int port, string playerName){

		this.playerName = playerName;

		bool useNat = !Network.HavePublicAddress();
		Network.InitializeServer(players, port, useNat);
		MasterServer.RegisterHost(gameName, gameName, gameName);
	}
	
	public void connect(string ip, string playerName){

		this.playerName = playerName;

		Debug.Log("Connecting to "+ip+":"+serverPort);
		Network.Connect(ip, serverPort);		
		nowConnecting=true;	
	}

	public void connect(string[] ip, string playerName){

		this.playerName = playerName;

		Debug.Log("Connecting to "+ip+":"+serverPort);
		Network.Connect(ip, serverPort);		
		nowConnecting=true;	
	}

	public void disconnect(){
		Network.Disconnect();
		mpm.playerList.Clear();
	}

	public void disconnectServer(){
		Network.Disconnect();
		MasterServer.UnregisterHost();
		mpm.playerList.Clear();
	}

	public void fetchHostList(){

		if (MasterServer.PollHostList().Length != 0) {
			hostData = MasterServer.PollHostList();
			int i = 0;
			while (i < hostData.Length) {
				Debug.Log("Game name: " + hostData[i].gameName);
				i++;
			}
			MasterServer.ClearHostList();
		}
	}

	
	public PlayerNode GetPlayerNode(NetworkPlayer networkPlayer){
		foreach(PlayerNode entry in mpm.playerList){
			if(entry.networkPlayer==networkPlayer){
				return entry;
			}
		}
		Debug.LogError("GetPlayerNode: Requested a playernode of non-existing player!");
		return null;
	}
		
	public void loadLevel(){
		networkView.RPC( "loadNetworkLevel", RPCMode.All, "TestLevel", lastLevelPrefix + 1);
	}
	
	[RPC]
	public void loadNetworkLevel(string level, int levelPrefix){

		lastLevelPrefix = levelPrefix;
		Network.SetSendingEnabled(0, false);
		Network.isMessageQueueRunning = false;

		Network.SetLevelPrefix(levelPrefix);
		Application.LoadLevel(level);

		Network.isMessageQueueRunning = true;
		// Now the level has been loaded and we can start sending out data to clients
		Network.SetSendingEnabled(0, true);
	
	}
	
	[RPC]
	public void updateTeam(NetworkPlayer player, string team){

		foreach(PlayerNode element in mpm.playerList){
			if(element.networkPlayer == player){
				element.team = team;
				return;
			}
		}

	}

	[RPC]
	public void sendPlayerName(string name, string team, NetworkMessageInfo info){
		PlayerNode newEntry = new PlayerNode(name, info.sender, team);
		mpm.playerList.Add(newEntry);
	}

	public void OnServerInitialized(){
		PlayerNode newPlayer = new PlayerNode(playerName, Network.player, "A");
		mpm.playerList.Add(newPlayer);
		networkView.RPC("sendPlayerName", RPCMode.OthersBuffered, playerName, "A");
	}

	public void OnConnectedToServer(){
		PlayerNode newPlayer = new PlayerNode(playerName, Network.player, "B");
		mpm.playerList.Add(newPlayer);
		networkView.RPC ("sendPlayerName", RPCMode.Server, playerName, "B");
	}

	public void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.Log("Player disconnected from: " + player.ipAddress+":" + player.port);
		mpm.playerList.Remove( GetPlayerNode(player) );
	}



	public void OnFailedToConnectToMasterServer(NetworkConnectionError info)
	{
		Debug.Log("FailedToConnect to MasterServer info:"+info);
	}
	
	public void OnFailedToConnect(NetworkConnectionError info)
	{
		Debug.Log("FailedToConnect info:"+info);
	}

}
