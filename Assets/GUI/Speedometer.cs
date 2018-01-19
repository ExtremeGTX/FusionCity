using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Speedometer : MonoBehaviour {


[SerializeField] Text kphDisplay ;
[SerializeField] Text AngDisplay ;
[SerializeField] GameObject Vehicle; 
Rigidbody rb;
 
	// Use this for initialization
	void Start () {
		rb = Vehicle.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
   		double kph  = rb.velocity.magnitude * 3.6;
		float  angv = rb.angularVelocity.magnitude;
    	kphDisplay.text = ((int)(kph)).ToString("Speed: ##0 kph");
		AngDisplay.text = angv.ToString("AngSpeed: ##0.000 rad/s");
	}
}
