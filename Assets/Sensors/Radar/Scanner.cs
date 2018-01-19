using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using System.Threading;

//[RequireComponent(typeof(UdpSender))]
public class Scanner : MonoBehaviour {
    [SerializeField] public string SensorID = "RL";
	public float range = 80;
	public float angle = 180;
	public float angularResolution = 1;
    public float frequence = 75;
	public bool hasNoise = true;
	public float systemError = 0.04f;
	
	public Color color = Color.blue;
	public static bool showDebugRay = true;

    public Text GUIAlert;
	private int numLines;
	private RaycastHit hit;

    private float distance;
    private float error;
    private StringBuilder builder;
	
    private List<GameObject> LineRendererGOList = new List<GameObject>();

    

    public Dictionary<int, Target> TargetList = new Dictionary<int, Target>();

    public int TotalDetectedTargets=0;
    public float TargetMinAngle=0;
    public float TargetMaxAngle=0;
    public float TargetCorrectedMinAngle=0;
    public float TargetCorrectedMaxAngle=0;
    Rigidbody rb;
    void InitLRList(int count)
    {
       while(count > LineRendererGOList.Count)
       {
            GameObject g = new GameObject();
            
            g.transform.parent = this.gameObject.transform;
            LineRenderer lr = g.AddComponent<LineRenderer>();
            lr.startWidth=0.01f;
            lr.endWidth=0.01f;
            
            lr.material = new Material(Shader.Find("Unlit/Color"));
            lr.material.color=Color.green;
            lr.startColor=Color.green; //Color need a basic material
            lr.endColor=Color.green;
            lr.receiveShadows = false;
            lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            LineRendererGOList.Add(g);
        }

        if (LineRendererGOList.Count > count && LineRendererGOList.Count > 0)
        {
            GameObject g = LineRendererGOList[LineRendererGOList.Count-1];
            Destroy(g);
            LineRendererGOList.RemoveAt(LineRendererGOList.Count-1);
        }

    }
	void Start() {
        rb = transform.parent.GetComponentInParent<Rigidbody>();
		numLines = (int)(angle / angularResolution) + 1;
        InitLRList(numLines);
	}
	
	void FixedUpdate() {
		range = Mathf.Clamp(range, 0.01f, range);
		angle = Mathf.Clamp(angle, 0.0f, angle);
		angularResolution = Mathf.Clamp(angularResolution, 0.01f, angle * 0.5f);
        numLines = (int)(angle / angularResolution) + 1;
        if (showDebugRay==false)
        {
            InitLRList(0);
        }
        else if (LineRendererGOList.Count != numLines)
        {
            InitLRList(numLines);
        }
    }

    void Update() 
    {
        bool Alert=false;
        //builder = new StringBuilder();
        TargetList.Clear();
        for (int index = 0; index < numLines; index++)
        {
            error = (hasNoise ? Noise() : 0.0f);
            distance = range + error;

            Vector3 ray = transform.rotation * Quaternion.AngleAxis(angle * 0.5f + (-1 * index * angularResolution), Vector3.up) * Vector3.forward;
            if (Physics.Raycast(transform.position, ray, out hit, range))
                distance = hit.distance + error;
            

            if (distance < range)
            {
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
                tp.angle = angle * 0.5f + (-1 * index * angularResolution);
                tp.distance = distance;
                tp.point = hit.point;
                t.name = hit.transform.name;
                //t.RelativePosition = Vector2.Distance(new Vector2(rb.position.x, rb.position.z), new Vector2(hit.rigidbody.position.x, hit.rigidbody.position.z));
                if (hit.rigidbody!=null)
                {
                    //Relative speed between host and target
                    //t.RelativeVelocity = hit.rigidbody.velocity - rb.velocity;
                    t.RelativeVelocity = rb.velocity.magnitude - hit.rigidbody.velocity.magnitude;
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

            if (showDebugRay)
            {
                LineRenderer lr =  LineRendererGOList[index].GetComponent<LineRenderer>();
                lr.name = "LR_"+(angle * 0.5f + (-1 * index * angularResolution)).ToString();
                lr.SetPosition( 0, transform.position );
                lr.SetPosition( 1,transform.position+(ray * distance));
                if (distance < range){
                    lr.startColor=Color.red; //Color need a basic material
                    lr.endColor=Color.red;
                    lr.material.color=Color.red;
                }
                else
                {
                    lr.startColor=Color.green; //Color need a basic material
                    lr.endColor=Color.green;
                    lr.material.color=Color.green;
                }
            //Debug.DrawLine(transform.position, transform.position+(ray * distance), Color.green,0.0f,true);
            //Debug.DrawRay(transform.position, ray * distance, Color.yellow);
            //Debug.DrawRay(transform.position, ray * distance, color);
                
            }
            //builder.AppendFormat("{0:0.000} ", distance);
         
        }
        //SendMessage("SetData", builder.ToString());
        TotalDetectedTargets = TargetList.Count;
        if (TotalDetectedTargets>0)
        {
            foreach (KeyValuePair<int,Target> item in TargetList)
            {
                //TargetMinAngle = item.Value.minAngle;
                //TargetMaxAngle = item.Value.maxAngle;
                

                Transform p = gameObject.transform.parent;
                
                TargetCorrectedMinAngle = TargetMinAngle + p.localEulerAngles.y;
                TargetCorrectedMaxAngle = TargetMaxAngle + p.localEulerAngles.y;
            }
        }
        if (Alert)
            GUIAlert.color = Color.red;
        else
            GUIAlert.color = Color.black;

        //if (SensorID=="RR")
            SendData();    
	}

	float Noise() {
		return Random.Range(-systemError, systemError);
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
                   + delimter + this.angle.ToString("##0.00")
                  
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
}
