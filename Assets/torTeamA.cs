using UnityEngine;
using System.Collections;

public class torTeamA : MonoBehaviour {

	private score score;
	private multiPlayerManager mp;

	public void Start(){
		score = (score)GameObject.FindGameObjectWithTag("score").GetComponent("score");
		mp = (multiPlayerManager) GameObject.FindGameObjectWithTag("mp").GetComponent("multiPlayerManager");
	}


	void OnCollisionEnter(Collision collision) {
		foreach (ContactPoint contact in collision.contacts) {
			Debug.DrawRay(contact.point, contact.normal, Color.white);
		}

		score.tor ("A");	
		mp.reset();
	}
}
