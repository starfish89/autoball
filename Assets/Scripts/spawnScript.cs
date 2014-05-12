using UnityEngine;
using System.Collections;

public class spawnScript : MonoBehaviour {

	public Transform playerPrefab;
	public Camera cam;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	
	public void OnLevelWasLoaded(int level) {
		Spawnplayer();
	}
	
	public void Spawnplayer(){	
		Transform myNewTrans = (Transform)Network.Instantiate(playerPrefab, transform.position + new Vector3(0,0,0), transform.rotation, 0);		
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
		
		/* 
	* Note that we only remove our own objects, but we cannot remove the other players 
	* objects since we don't know what they are; we didn't keep track of them. 
	* In a game you would usually reload the level or load the main menu level anyway ;).
	* 
	* In fact, we could use "Application.LoadLevel(Application.loadedLevel);" here instead to reset the scene.
	*/
		Application.LoadLevel(Application.loadedLevel);
	}
}
