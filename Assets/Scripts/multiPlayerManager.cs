using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class multiPlayerManager : MonoBehaviour {

	public List<PlayerNode> playerList = new List<PlayerNode>();


	public Transform playerPrefab;
	public Transform ballPrefab;
	public GameObject cam;

	private spawn spawn;

	private Transform ball;

	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad(this);
	}

	public void OnLevelWasLoaded(int level) {
	
		cam = GameObject.FindGameObjectWithTag("MainCamera");

		if(Network.isServer){


			spawn = new spawn(this);
			//Spawn Punkte finden
			spawn.findSpawnPoints();
			//Server player spawnen
			spawn.checkSpawn (playerList, Network.player, true);

			ball = (Transform)Network.Instantiate(ballPrefab, new Vector3(0,5,0), Quaternion.identity, 0);
			
		} else {
			// an den server senden dass wir uns verbunden haben
			networkView.RPC("playerConnected", RPCMode.Server, Network.player);
		}

	}

	//empfängt nur der server
	[RPC]
	public void playerConnected(NetworkPlayer player){
		//server spawnt neuen player
		spawn.checkSpawn (playerList, player, false);

	}

	[RPC]
	public void spawnPlayer(Vector3 position){
		Transform myNewTrans = (Transform)Network.Instantiate(playerPrefab, position, transform.rotation, 0);		
		follow f = (follow)cam.GetComponent("follow");
		f.target = myNewTrans;
		myNewTrans.LookAt(new Vector3(0,1,0));
	}

	public void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.Log("Clean up after player " + player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
	
	public void OnDisconnectedFromServer(NetworkDisconnection info) {
		Debug.Log("Clean up a bit after server quit");
		Network.RemoveRPCs(Network.player);
		Network.DestroyPlayerObjects(Network.player);
		
		Application.LoadLevel(Application.loadedLevel);
	}


	public void reset(){
		Debug.Log("reset");

		ball.position = new Vector3(0,2,0);
	}
}
