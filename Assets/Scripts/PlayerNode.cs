using UnityEngine;

public class PlayerNode
{
	public string playerName;
	public string team;
	public NetworkPlayer networkPlayer;
	public Vector3 spawnPoint;

	public PlayerNode (string playerName, NetworkPlayer nplayer, string team)
	{
		this.playerName = playerName;
		this.networkPlayer = nplayer;
		this.team = team;
	}

	public string changeTeam(){
		if(team.Equals("A")){
			this.team = "B";
			return "B";
		} else {
			this.team = "A";
			return "A";
		}
	}

}

