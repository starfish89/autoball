using UnityEngine;
using System.Collections;

public class RotateScript : MonoBehaviour {

	//Einfach rotieren lassen
	void FixedUpdate(){
		this.transform.Rotate(0.2f, 0, 1, Space.Self);
	}
}
