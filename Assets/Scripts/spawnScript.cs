using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class spawnScript : MonoBehaviour {

	public Transform playerPrefab;
	public Transform ballPrefab;
	public Camera cam;

	List<Vector3> spawnPointListA = new List<Vector3>();
	List<Vector3> spawnPointListB = new List<Vector3>();

	private Vector3 ballPosition = new Vector3(0,20,0);
	

	void Update(){
		if(Input.GetKeyUp("e")){
			Transform ball = (Transform)Network.Instantiate(ballPrefab, ballPosition, Quaternion.identity, 0);
		}
	}

	public void OnLevelWasLoaded(int level) {

		if(Network.isServer){

			//Team A
			foreach(GameObject o in GameObject.FindGameObjectsWithTag("spawnPointA")){
				Transform trans = o.transform;
				Vector3 pos = trans.TransformPoint(trans.position);
				spawnPointListA.Add(pos);
			}
			//Team B
			foreach(GameObject o in GameObject.FindGameObjectsWithTag("spawnPointB")){
				Transform trans = o.transform;
				Vector3 pos = trans.TransformPoint(trans.position);
				spawnPointListB.Add(pos);
			}

			spawn (new Vector3(10,1,0));
			Transform ball = (Transform)Network.Instantiate(ballPrefab, ballPosition, Quaternion.identity, 0);

		} else {
			networkView.RPC("playerConnected", RPCMode.Server, Network.player);
		}
	}

	[RPC]
	public void playerConnected(NetworkPlayer player){
		networkView.RPC ("spawn", player, new Vector3(-10,1,0));
	}

	[RPC]
	public void spawn(Vector3 position){
		Transform myNewTrans = (Transform)Network.Instantiate(playerPrefab, position, transform.rotation, 0);		
		follow f = (follow)cam.GetComponent("follow");
		f.target = myNewTrans;

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
