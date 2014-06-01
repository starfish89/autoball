using UnityEngine;
using System.Collections;

public class score : MonoBehaviour {

	public GUISkin fontLeft;
	public GUISkin fontRight;

	private int teamA = 0;
	private int teamB = 0;

	public void tor(string team){
		if(team.Equals("A")){
			teamA++;
		} else {
			teamB++;
		}
	}

	public void OnGUI(){
		GUI.skin = fontLeft;
		GUI.Label(new Rect(50,0,Screen.width/2,50), "Team A: "+teamA);
		GUI.skin = fontRight;
		GUI.Label (new Rect(Screen.width/2, 0, Screen.width/2-50,50), "Team B: "+teamB);
	}
}
