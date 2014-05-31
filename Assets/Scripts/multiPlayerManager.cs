using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class multiPlayerManager : MonoBehaviour {

	public List<PlayerNode> playerList = new List<PlayerNode>();
	public List<Vector3> spawnPointListA = new List<Vector3>();
	public List<Vector3> spawnPointListB = new List<Vector3>();

	public Transform playerPrefab;
	public Transform ballPrefab;
	public GameObject cam;


	private int spawnACount = 0;
	private int spawnBCount = 0;
	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad(this);
	}

	public void OnLevelWasLoaded(int level) {

		cam = GameObject.FindGameObjectWithTag("MainCamera");

		if(Network.isServer){
			
			//Team A
			foreach(GameObject o in GameObject.FindGameObjectsWithTag("spawnPointA")){
				Transform trans = o.transform;
				//Vector3 pos = trans.TransformPoint(trans.position);
				spawnPointListA.Add(trans.position);
			}
			//Team B
			foreach(GameObject o in GameObject.FindGameObjectsWithTag("spawnPointB")){
				Transform trans = o.transform;
				//Vector3 pos = trans.TransformPoint(trans.position);
				spawnPointListB.Add(trans.position);
			}

			foreach(Vector3 bla in spawnPointListA){
				Debug.Log (bla.x + ":" + bla.y);
			}


			checkSpawn (Network.player, true);

			Transform ball = (Transform)Network.Instantiate(ballPrefab, new Vector3(0,5,0), Quaternion.identity, 0);
			
		} else {
			// an den server senden dass wir uns verbunden haben
			networkView.RPC("playerConnected", RPCMode.Server, Network.player);
		}

	}

	public void checkSpawn(NetworkPlayer player, bool server){

		foreach (PlayerNode node in playerList){
			if(node.networkPlayer == player){
				
				if(node.team.Equals("A")){
					if(server){
						spawn (spawnPointListA[spawnACount]);
					} else {
						networkView.RPC ("spawn", player, spawnPointListA[spawnACount]);
					}
					spawnACount++;
				} else {
					if(server){
						spawn (spawnPointListB[spawnBCount]);
					} else {
						networkView.RPC ("spawn", player, spawnPointListB[spawnBCount]);
					}
					spawnBCount++;
				}
				
				break;
			}
		}
	
	}

	//empfängt nur der server
	[RPC]
	public void playerConnected(NetworkPlayer player){
		//server spawnt neuen player
		checkSpawn (player, false);

	}
	
	[RPC]
	public void spawn(Vector3 position){
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

}
