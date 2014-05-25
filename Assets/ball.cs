using UnityEngine;
using System.Collections;

public class ball : MonoBehaviour {

	private Quaternion oldRot;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {


		//transform.position = Vector3.Lerp( transform.position, readNetworkPos, 10f * Time.deltaTime );
		transform.rotation = Quaternion.Slerp(transform.rotation, oldRot, 10f * Time.deltaTime);
		oldRot = transform.rotation;
	}

}
