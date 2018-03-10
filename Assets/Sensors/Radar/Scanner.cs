using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System;

//[RequireComponent(typeof(UdpSender))]
public class Scanner : MonoBehaviour,ISensor {
    [SerializeField] public string SensorID = "RL";
	public float range = 80;
	public float FieldOfView = 180;
	public float angularResolution = 1;
    public float ScanningFrequency = 75;
	public bool hasNoise = true;
	public float systemError = 0.04f;

    public Text GUIAlert;

	private int NoOfBeams;

	private RaycastHit hit;

    private float error;

    public Action<int,Vector3,Vector3,bool> OnSensorBeamUpdate { get; set; }
    public Action OnSensorInitDone { get; set; }
    public Action OnSensorUpdate { get; set ; }

    public Dictionary<int, Target> TargetList = new Dictionary<int, Target>();

    private List<BeamInfo> BeamsInfo = new List<BeamInfo>();

    public int dbgTotalDetectedTargets=0;
    public float dbgTargetMinAngle=0;
    public float dbgTargetMaxAngle=0;
    public float dbgTargetCorrectedMinAngle=0;
    public float dbgTargetCorrectedMaxAngle=0;
    Rigidbody rb;
    
	void Start() {
        rb = transform.parent.GetComponentInParent<Rigidbody>();
		NoOfBeams = (int)(FieldOfView / angularResolution) + 1;
        
        if (OnSensorInitDone != null)
            OnSensorInitDone();
	}
	
	private void UnusedFixedUpdate() {
		// range = Mathf.Clamp(range, 0.01f, range);
		// FieldOfView = Mathf.Clamp(FieldOfView, 0.0f, FieldOfView);
		// angularResolution = Mathf.Clamp(angularResolution, 0.01f, FieldOfView * 0.5f);
        NoOfBeams = (int)(FieldOfView / angularResolution) + 1;
        
         if (OnSensorUpdate != null)
            OnSensorUpdate();
    }

    void LateUpdate()
    {
        if (BeamsInfo.Count==0)
            return;

        for (int index=0;index<NoOfBeams;index++){
            if (OnSensorBeamUpdate != null)
                OnSensorBeamUpdate(index,
                                    transform.position,
                                    transform.position+BeamsInfo[index].Ray, 
                                    BeamsInfo[index].Alert
                                   );
        }    

    }
    
    int mcc_start=0;
    void FixedUpdate() 
    {
        if(mcc_start<2)
        {
            mcc_start++;
            return;
        }
        mcc_start=0;
        BeamsInfo.Clear();
        float distance=0;
        NoOfBeams = (int)(FieldOfView / angularResolution) + 1;
        
         if (OnSensorUpdate != null)
            OnSensorUpdate();

        bool Alert=false;
        TargetList.Clear();
        for (int index = 0; index < NoOfBeams; index++)
        {
            error = (hasNoise ? Noise() : 0.0f);
            distance = range + error;

            Vector3 ray = transform.rotation * Quaternion.AngleAxis(FieldOfView * 0.5f + (-1 * index * angularResolution), Vector3.up) * Vector3.forward;
            if (Physics.Raycast(transform.position, ray, out hit, range))
                distance = hit.distance + error;
            
            if (distance < range)
            {
                /* Future Implementation, Neglect anything not a Car */
                /*if (!hit.transform.gameObject.name.Contains("Car"))
                    continue;*/

                Alert = true;
                int goid = hit.transform.gameObject.GetInstanceID();
                Target t;
                if (!TargetList.ContainsKey(goid))
                {
                    t = new Target();
                    TargetList.Add(goid,t);
                }

                t = TargetList[goid];
                Target.PointData tp = new Target.PointData();
                tp.angle = FieldOfView * 0.5f + (-1 * index * angularResolution);
                tp.distance = distance;
                tp.point = hit.point;
                t.name = hit.transform.name;
                //t.RelativePosition = Vector2.Distance(new Vector2(rb.position.x, rb.position.z), new Vector2(hit.rigidbody.position.x, hit.rigidbody.position.z));
                if (hit.rigidbody!=null)
                {
                    //Relative speed between host and target
                    //t.RelativeVelocity = hit.rigidbody.velocity - rb.velocity;
                    float Dir = (rb.velocity.x - hit.rigidbody.velocity.x) < 0.0f ? -1.0f : 1.0f;// GetDirection(rb.velocity, hit.rigidbody.velocity);
                    Dir *= (rb.velocity.z - hit.rigidbody.velocity.z) < 0.0f ? -1.0f : 1.0f;
                    t.RelativeVelocity = Mathf.Abs(rb.velocity.magnitude - hit.rigidbody.velocity.magnitude)*Dir;
                    //t.RelativeVelocity = (rb.velocity - hit.rigidbody.velocity).normalized.magnitude;
                    
                    //Relative Speed between host and target with projection on hit ray
                    tp.RelativeVelocity = hit.rigidbody.velocity - rb.velocity;
                    tp.RelativeVelocity = Vector3.Project(tp.RelativeVelocity,ray);
                }
                else
                { 
                    tp.RelativeVelocity= new Vector3(0,0,0);
                }
                t.yawRate = hit.rigidbody ? hit.rigidbody.angularVelocity.magnitude:0.0f;
                t.PointsData.Add(tp);
            }
            BeamsInfo.Add(new BeamInfo((ray*distance),(distance<range)));
            
        
        }
#if _dbg_
        dbgTotalDetectedTargets = TargetList.Count;
        if (dbgTotalDetectedTargets>0)
        {
            foreach (KeyValuePair<int,Target> item in TargetList)
            {
                //TargetMinAngle = item.Value.minAngle;
                //TargetMaxAngle = item.Value.maxAngle;
                

                Transform p = gameObject.transform.parent;
                
                dbgTargetCorrectedMinAngle = dbgTargetMinAngle + p.localEulerAngles.y;
                dbgTargetCorrectedMaxAngle = dbgTargetMaxAngle + p.localEulerAngles.y;
            }
        }
#endif
        if (Alert)
            GUIAlert.color = Color.red;
        else
            GUIAlert.color = Color.black;

            SendData();    
	}

	float Noise() {
		return UnityEngine.Random.Range(-systemError, systemError);
	}

    public void setRange(float val)
    {
        range = System.Convert.ToInt32(val);
    }

    public void SendData() 
    {
        string delimter =" | ";
        Text tTargets = GameObject.Find("targets"+SensorID).GetComponent<Text>();
        Text tStat = GameObject.Find("stat"+SensorID).GetComponent<Text>();

        
            

        RadarManager rm =  gameObject.GetComponentInParent<RadarManager>();
        if (!rm)
            return;
       

        
        string s   =  SensorID.ToString()
                   /* Host Data */
                   + delimter + rb.velocity.magnitude.ToString("##0.000")
                   + delimter + rb.angularVelocity.magnitude.ToString("##0.000")
                   + delimter + rb.transform.position.x.ToString("##0.000")
                   + delimter + rb.transform.position.z.ToString("##0.000")
                   /* Sensor Data */
                   + delimter + transform.parent.transform.localEulerAngles.y.ToString("000.0")
                   + delimter + this.range.ToString("##0.00")
                   + delimter + this.FieldOfView.ToString("##0.00")
                  
                   ;
        
        tStat.text = s;

        if (TargetList.Count==0)
        {
            tTargets.text="";
            return;
        }

        string ts="";
        int tCount=0;
        foreach (KeyValuePair<int,Target> t in TargetList)
        {
            ts = ts + tCount.ToString()+":"+ t.Value.ToString();
            tCount++;
        }
        tTargets.text = ts;
    }
    void GetDirection(Vector3 a, Vector3 b)
    {
        Debug.Log(string.Format("DirX:{0} | DirZ:{1}",a.x-b.x,a.z-b.z));
    }

    public int getNoOfBeams()
    {
        return NoOfBeams;
    }

    public float getRange()
    {
        return range;
    }
}
