using UnityEngine;

public class PlayerNode
{
	public string playerName;
	public NetworkPlayer networkPlayer;

	public PlayerNode (string playerName, NetworkPlayer nplayer)
	{
		this.playerName = playerName;
		this.networkPlayer = nplayer;
	}

}

