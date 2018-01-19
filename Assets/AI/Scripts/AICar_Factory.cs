using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICar_Factory : MonoBehaviour {

	
	[SerializeField] GameObject AICar;
	[SerializeField] float Angle=0;
    static int VehicleID=0;
	void Start () {
		GameObject g = Instantiate(AICar) as GameObject;
		g.transform.position = gameObject.transform.position;
		//g.transform.rotation = gameObject.transform.rotation;
		g.transform.Rotate(Vector3.up,Angle);
		g.transform.name = "Car("+ VehicleID++ +")";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
