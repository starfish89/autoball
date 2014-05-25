using UnityEngine;
using System.Collections;

public class ball : MonoBehaviour {

	private Vector3 readNetworkPos;
	private Quaternion readNetworkRot;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//Position updaten
		//transform.position = Vector3.Lerp( transform.position, readNetworkPos, 10f * Time.deltaTime );
		//transform.rotation = Quaternion.Slerp(transform.rotation, readNetworkRot, 10f * Time.deltaTime);

	}

	//Netzwerk Daten
	void OnSerializeNetworkView( BitStream stream )
	{
		Vector3 pos;
		Quaternion rot;
		// writing information, push current paddle position
		if( stream.isWriting )
		{
			pos = transform.position;
			rot = transform.rotation;
			stream.Serialize( ref pos );
			stream.Serialize( ref rot );
		}
		// reading information, read paddle position
		else
		{
			pos = Vector3.zero;
			rot = Quaternion.identity;
			stream.Serialize( ref pos );
			stream.Serialize( ref rot );
			readNetworkPos = pos;
			readNetworkRot = rot;
		}
	}
}
