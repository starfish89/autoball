using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class spawn : MonoBehaviour {

	public List<Vector3> spawnPointListA = new List<Vector3>();
	public List<Vector3> spawnPointListB = new List<Vector3>();

	private multiPlayerManager mp;

	private int spawnACount = 0;
	private int spawnBCount = 0;

	public spawn(multiPlayerManager mp){
		this.mp = mp;
	}

	public void findSpawnPoints(){
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
	}

	public void checkSpawn(List<PlayerNode> playerList, NetworkPlayer player, bool server){
		
		foreach (PlayerNode node in playerList){
			if(node.networkPlayer == player){
				
				if(node.team.Equals("A")){
					if(server){
						mp.spawnPlayer (spawnPointListA[spawnACount]);
					} else {
						networkView.RPC ("spawnPlayer", player, spawnPointListA[spawnACount]);
					}
					spawnACount++;
				} else {
					if(server){
						mp.spawnPlayer (spawnPointListB[spawnBCount]);
					} else {
						networkView.RPC ("spawnPlayer", player, spawnPointListB[spawnBCount]);
					}
					spawnBCount++;
				}
				
				break;
			}
		}
		
	}

}
