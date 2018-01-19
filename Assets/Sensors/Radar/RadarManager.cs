using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class RadarManager : MonoBehaviour {
	[SerializeField] private GameObject[] RadarObjs = new GameObject[4];

	[SerializeField] private Text[] AlertObjs = new Text[4];

	[SerializeField] Text FrontSensorStats;
	[SerializeField] Text RearSensorStats;

	[SerializeField] public UdpSender udpSender;
	// Use this for initialization
	void Start () {

		/* 
		 * Search List of Objects for GUI Component of Type Text
		 * which will display sensor settings
		 */
		FrontSensorStats = GameObject.Find("F_Range").GetComponent<Text>();
		RearSensorStats = GameObject.Find("R_Range").GetComponent<Text>();

		for (int i=0;i<RadarObjs.Length;i++)
		{
			Scanner s = RadarObjs[i].GetComponent<Scanner>();
			s.GUIAlert = AlertObjs[i];
			udpQueue.Add(new Queue(20,5));
		}	
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ChangeSensorSetup(int selection)
	{
		switch	(selection)
		{
			case 0:
				RadarObjs[0].SetActive(false);
				RadarObjs[1].SetActive(false);
				RadarObjs[2].SetActive(true);
				RadarObjs[3].SetActive(true);

				AlertObjs[0].color = Color.clear;
				AlertObjs[1].color = Color.clear;
			break;

			case 1:
				RadarObjs[0].SetActive(true);
				RadarObjs[1].SetActive(true);
				RadarObjs[2].SetActive(true);
				RadarObjs[3].SetActive(true);
			break;
			
			case 2:
				RadarObjs[0].SetActive(false);
				RadarObjs[1].SetActive(false);
				RadarObjs[2].SetActive(false);
				RadarObjs[3].SetActive(false);

				foreach (Text item in AlertObjs)
				{
					item.color = Color.clear;
				}

			break;
			
		}
	}

	public void SetSensorFront_MountAngle(float val)
	{
		Vector3 angles = RadarObjs[1].transform.parent.transform.localEulerAngles;
		angles.y = val;
		RadarObjs[1].transform.parent.transform.localEulerAngles=angles;

		angles = RadarObjs[0].transform.parent.transform.localEulerAngles;
		angles.y = -1*val;
		RadarObjs[0].transform.parent.transform.localEulerAngles=angles;

		UpdateFrontSensor_Stats();
	}
	public void SetSensorFront_Range(float val)
	{
		Scanner s = RadarObjs[0].GetComponent<Scanner>();
		Scanner s1 = RadarObjs[1].GetComponent<Scanner>();
		s.range = s1.range = System.Convert.ToInt32(val);

		UpdateFrontSensor_Stats();
	}

	public void SetSensorFront_FoV(float val)
	{
		Scanner s = RadarObjs[0].GetComponent<Scanner>();
		Scanner s1 = RadarObjs[1].GetComponent<Scanner>();
		s.angle = s1.angle = System.Convert.ToInt32(val);

		UpdateFrontSensor_Stats();
	}

	public void SetSensorRear_Range(float val)
	{
		Scanner s2 = RadarObjs[2].GetComponent<Scanner>();
		Scanner s3 = RadarObjs[3].GetComponent<Scanner>();
		s2.range = s3.range = System.Convert.ToInt32(val);
		UpdateRearSensor_Stats();
	}

	public void SetSensorRear_FoV(float val)
	{
		Scanner s2 = RadarObjs[2].GetComponent<Scanner>();
		Scanner s3 = RadarObjs[3].GetComponent<Scanner>();
		s2.angle = s3.angle = System.Convert.ToInt32(val);
		UpdateRearSensor_Stats();
	}

	public void SetSensorRear_MountAngle(float val)
	{
		Vector3 angles = RadarObjs[2].transform.parent.transform.localEulerAngles;
		angles.y = val;
		RadarObjs[2].transform.parent.transform.localEulerAngles=angles;

		angles = RadarObjs[3].transform.parent.transform.localEulerAngles;
		angles.y = -1*val;
		RadarObjs[3].transform.parent.transform.localEulerAngles=angles;
		UpdateRearSensor_Stats();
	}

	public void UpdateFrontSensor_Stats()
	{
		Scanner s = RadarObjs[0].GetComponent<Scanner>();
		FrontSensorStats.text = string.Format("Range: {0}\nFoV: {1}\nAngle: {2}",s.range,s.angle,RadarObjs[1].transform.parent.transform.localEulerAngles.y);
	}
	public void UpdateRearSensor_Stats()
	{
		Scanner s = RadarObjs[2].GetComponent<Scanner>();
		RearSensorStats.text = string.Format("Range: {0}\nFoV: {1}\nAngle: {2}",s.range,s.angle,RadarObjs[2].transform.parent.transform.localEulerAngles.y);
	}

	public void DisplayBeams(bool enable)
	{
		Scanner.showDebugRay = enable;
	}

	static List<Queue> udpQueue = new List<Queue>();
	bool sensorDataUpdated = false;
	public UdpSender GetUdpSocket()
	{
		return udpSender;
	}

	public void SetData(string data)
	{
		udpSender.SetData(data);
	}
}
