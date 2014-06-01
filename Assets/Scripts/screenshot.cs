using UnityEngine;
using System.Collections;
	
	public class screenshot : MonoBehaviour {
		
	public void Update(){

		if(Input.GetKeyUp("k")){
			Application.CaptureScreenshot("Screenshot.png", 2);
		}

	}

	}
