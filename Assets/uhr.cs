using UnityEngine;
using System.Collections;

public class uhr : MonoBehaviour {

	public GUISkin skin;
	private int time = 5*60;

	// Use this for initialization
	void Start () {
		InvokeRepeating("counterMinus", 1, 1f);
	}

	public void counterMinus(){
		time--;
	}


	public void OnGUI(){

		if(time<=0){
			CancelInvoke();

			//TODO
			//Spiel ENDE
		}

		GUI.skin = skin;
		GUI.Label(new Rect(Screen.width/2-50,0,100,50), getTime());
	}

	private string getTime(){

		int minuten = time/60;
		int sekunden = time%60;

		string sekundenString;
		if(sekunden<10){
			sekundenString = "0" + sekunden;
		} else {
			sekundenString = "" + sekunden;
		}

		return minuten + ":" + sekundenString;
	}
}
