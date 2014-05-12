﻿#pragma strict

//Wagensteuerung
var flWheelCollider: WheelCollider;
var frWheelCollider: WheelCollider;
var rlWheelCollider: WheelCollider;
var rrWheelCollider: WheelCollider;
var maxTorque = 260.0;	//Max Geschwindigkeit / Drehmoment
var maxBreakeTorque = 500; //Bremskraft
var maxSpeed: float = 200;
var maxBackwardSpeed: float = 40;
private var currentSpeed = 0.0;
var isBraking: boolean = false;
var Bremsstaerke : float = 5000;	//Vollbremsung
private var oldForwardFriction : float = 0.00;
private var oldSidewaysFriction : float = 0.00;
private var newForwardFriction : float = 0.04;
private var newSidewaysFriction : float = 0.01;
private var stopForwardFriction : float = 1;
private var stopSidewaysFriction : float = 1;

var CenterOfMass : Transform;
//Tacho
var GuiSpeedPointer : Texture2D;
var GuiSpeedDisplay : Texture2D;

//Reifenvisualisierung
var flWheel : Transform;
var frWheel : Transform;
var rlWheel : Transform;
var rrWheel : Transform;

//Gänge
var gearSpeed : int [];
private var currentGear: int = 0;


function Start () {
	rigidbody.centerOfMass = CenterOfMass.localPosition;
	Debug.Log(rigidbody.centerOfMass);
	oldForwardFriction = frWheelCollider.forwardFriction.stiffness;
	oldSidewaysFriction = frWheelCollider.sidewaysFriction.stiffness;
	
}

function FixedUpdate () {
	if(networkView.isMine){
		//Speed berechnen
		currentSpeed = (Mathf.PI * 2 * flWheelCollider.radius) * flWheelCollider.rpm * 60 / 1000;
		currentSpeed = Mathf.Round(currentSpeed);
		
		// Wollen wir bremsen?
		if(((currentSpeed > 0) && (Input.GetAxis("Vertical") < 0)) || ((currentSpeed < 0) && (Input.GetAxis("Vertical") > 0))){
			isBraking = true;
		}
		else{
			isBraking = false;
			flWheelCollider.brakeTorque = 0;
			frWheelCollider.brakeTorque = 0;
		}
		
		//Beschleunigen mit max Geschwindigkeit und bremsen
		if (isBraking == false){	
			if ((currentSpeed < maxSpeed) && (currentSpeed > (maxBackwardSpeed*-1))){ 
				flWheelCollider.motorTorque = maxTorque * Input.GetAxis("Vertical");		//Drehmoment * Richtungsvektor (Vor/Rückwärts)
				frWheelCollider.motorTorque = maxTorque * Input.GetAxis("Vertical");
			}
			else{
				flWheelCollider.motorTorque = 0;
				frWheelCollider.motorTorque = 0;
			}
		}
		else{
			flWheelCollider.brakeTorque = maxBreakeTorque;
			frWheelCollider.brakeTorque = maxBreakeTorque;
			flWheelCollider.motorTorque = 0;
			frWheelCollider.motorTorque = 0;
		}
		
		//Max Lenkeinschlag (10 Grad)
		flWheelCollider.steerAngle = 10 * Input.GetAxis("Horizontal");
		frWheelCollider.steerAngle = 10 * Input.GetAxis("Horizontal");
		
		
		//Motorsound
		setCurrentGear();
		motorSound();
		
		//Vollbremsung
		vollbremsung();
		
		//Ohne Gas langsamer werden
		slower();
	}
}

//Nach jedem Framewechsel
function Update(){ 
	RotateWheels();
//	SteelWheels();
}

function RotateWheels(){
	//Räder drehen
	flWheel.Rotate(flWheelCollider.rpm / 60 * 360 * Time.deltaTime,0,0);
	frWheel.Rotate(-1*flWheelCollider.rpm / 60 * 360 * Time.deltaTime,0,0);
	rlWheel.Rotate(flWheelCollider.rpm / 60 * 360 * Time.deltaTime,0,0);
	rrWheel.Rotate(-1*flWheelCollider.rpm / 60 * 360 * Time.deltaTime,0,0);

}

//function SteelWheels(){
//	flWheel.localEulerAngles.y = flWheelCollider.steerAngle - flWheel.localEulerAngles.z;
//	frWheel.localEulerAngles.y = frWheelCollider.steerAngle - frWheel.localEulerAngles.z;
//	
//}

function setCurrentGear(){
	var gearNumber: int;
	gearNumber = gearSpeed.length;
	
	for(var i=0; i <gearNumber; i++){
		if(gearSpeed[i] > currentSpeed){
			currentGear = i;
			break;
		}		
	}	
} 

function motorSound(){
	var tempMinSpeed : float = 0.0;
	var tempMaxSpeed : float = 0.0;
	var currentPitch : float = 0.0;
	
	switch (currentGear){
		case 0:
			tempMinSpeed = 0.0;
			tempMaxSpeed = gearSpeed[currentGear];
			break;
		
		default:
			tempMinSpeed = gearSpeed[currentGear - 1];
			tempMaxSpeed = gearSpeed[currentGear];
	}
	
	currentPitch = ((Mathf.Abs(currentSpeed) - tempMinSpeed)/(tempMaxSpeed - tempMinSpeed)) + 0.8;
	audio.pitch = currentPitch;
}

function vollbremsung(){
	if(Input.GetKey("space")){
		rlWheelCollider.brakeTorque = Bremsstaerke;
		rrWheelCollider.brakeTorque = Bremsstaerke;
		
		if((Mathf.Abs(rigidbody.velocity.z) > 1) || (Mathf.Abs(rigidbody.velocity.x) > 1)){
			setFriction(newForwardFriction, newSidewaysFriction);
		}
		else{
			setFriction(stopForwardFriction, stopSidewaysFriction);
		}
	}
	else
	{
		rlWheelCollider.brakeTorque = 0;
		rrWheelCollider.brakeTorque = 0;
		setFriction(oldForwardFriction, oldSidewaysFriction);
	}
}

function setFriction(MyForwardFriction:float, MySidewaysFriction:float){
	frWheelCollider.forwardFriction.stiffness = MyForwardFriction;
	flWheelCollider.forwardFriction.stiffness = MyForwardFriction;
	rrWheelCollider.forwardFriction.stiffness = MyForwardFriction;
	rlWheelCollider.forwardFriction.stiffness = MyForwardFriction;

	frWheelCollider.sidewaysFriction.stiffness = MySidewaysFriction;
	flWheelCollider.sidewaysFriction.stiffness = MySidewaysFriction;
	rrWheelCollider.sidewaysFriction.stiffness = MySidewaysFriction;
	rlWheelCollider.sidewaysFriction.stiffness = MySidewaysFriction;

}

function slower (){
	var throttle = Input.GetAxis("Vertical");
	
	if(throttle == 0){
		rigidbody.drag = 0.25;
	}
	else{
		rigidbody.drag = 0;
	}
}

//Tacho wird "gezeichnet"
function OnGUI() {
	var pointerPosition : float = 40.0;
	
	GUI.Box(Rect(0.0,0.0,140.0,140.0),GuiSpeedDisplay);
	//Tacho malen und dann Koordinatensystem drehen.
	if(currentSpeed > 0){
		pointerPosition = currentSpeed + 40;
	}
	GUIUtility.RotateAroundPivot(pointerPosition,Vector2(70,70));
	GUI.DrawTexture(Rect(0.0,0.0,140.0,140.0),GuiSpeedPointer,ScaleMode.StretchToFill,true,0);
	
}