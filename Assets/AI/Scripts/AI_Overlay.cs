using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Overlay : MonoBehaviour {

	const string postiveFormat  = " 000.000";
	const string negativeFormat = "-000.000";
	const string numberFormat = " 000.000;-000.000";

	[SerializeField]
	TextMesh text;

	Rigidbody rb;
	// Use this for initialization
	void Start () {
		rb = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 V = transform.InverseTransformDirection(rb.velocity);
		Vector3 Av = transform.InverseTransformDirection(rb.angularVelocity);
		text.text = string.Format("V :{0:"+numberFormat+"}\n ({1:"+numberFormat+"}),({2:"+numberFormat+"})\n",V.magnitude,V.z,V.x)
				  + string.Format("Av:{0:"+numberFormat+"}\n ({1:"+numberFormat+"}),({2:"+numberFormat+"})",Av.magnitude,Av.z,Av.x);
	}
}
