using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class multiPlayerScript : MonoBehaviour {

	
	public string gameName = "AutoBall";
	public int serverPort =  6666;
	private string playerName = "";
	public HostData[] hostData = new HostData[0];
	
	public List<PlayerNode> playerList = new List<PlayerNode>();
	private int lastLevelPrefix = 0;
	public bool nowConnecting = false;

	public void Awake(){
		requestHost();
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
		playerList.Clear();
	}

	public void disconnectServer(){
		Network.Disconnect();
		MasterServer.UnregisterHost();
		playerList.Clear();
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
		foreach(PlayerNode entry in playerList){
			if(entry.networkPlayer==networkPlayer){
				return entry;
			}
		}
		Debug.LogError("GetPlayerNode: Requested a playernode of non-existing player!");
		return null;
	}
	

	[RPC]
	public void sendPlayerName(string name, NetworkMessageInfo info){
		PlayerNode newEntry = new PlayerNode(name, info.sender);
		playerList.Add(newEntry);
	}

	
	public void loadLevel(){
		networkView.RPC( "loadNetworkLevel", RPCMode.All, "TestLevel", lastLevelPrefix + 1);
	}
	
	[RPC]
	public void loadNetworkLevel(string level, int levelPrefix){
		
		lastLevelPrefix = levelPrefix;
		/*
		// There is no reason to send any more data over the network on the default channel,
		// because we are about to load the level, thus all those objects will get deleted anyway
		Network.SetSendingEnabled(0, false);	
		
		// We need to stop receiving because first the level must be loaded first.
		// Once the level is loaded, rpc's and other state update attached to objects in the level are allowed to fire
		Network.isMessageQueueRunning = false;
		
		// All network views loaded from a level will get a prefix into their NetworkViewID.
		// This will prevent old updates from clients leaking into a newly created scene.*/
		Network.SetLevelPrefix(levelPrefix);
		Application.LoadLevel(level);
		
		// Allow receiving data again
		Network.isMessageQueueRunning = true;
		// Now the level has been loaded and we can start sending out data to clients
		Network.SetSendingEnabled(0, true);
		
	}
	


	public void OnServerInitialized(){
		PlayerNode newPlayer = new PlayerNode(playerName, Network.player);
		playerList.Add(newPlayer);
		networkView.RPC("sendPlayerName", RPCMode.OthersBuffered, playerName);
	}

	public void OnConnectedToServer(){
		PlayerNode newPlayer = new PlayerNode(playerName, Network.player);
		playerList.Add(newPlayer);
		networkView.RPC ("sendPlayerName", RPCMode.Server, playerName);
	}
	
	public void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.Log("Player disconnected from: " + player.ipAddress+":" + player.port);
		playerList.Remove( GetPlayerNode(player) );
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
