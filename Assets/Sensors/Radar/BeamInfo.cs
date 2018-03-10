using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamInfo {
	public Vector3 Ray {get;private set;}
	public bool Alert {get;private set;}
	public BeamInfo(Vector3 ray, bool alert)
	{
		Ray = ray;
		Alert = alert;
	}
}
