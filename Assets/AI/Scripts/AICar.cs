using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AICar : MonoBehaviour
{
    public Vector3 centerOfMass;
    //List<Transform> path;
	List<Vector3> path;
    [SerializeField] Transform pathGroup;
    float maxSteer = 35.0f;
    [SerializeField] WheelCollider wheelFL;
    [SerializeField] WheelCollider wheelFR;
    [SerializeField] WheelCollider wheelRL;
    [SerializeField] WheelCollider wheelRR;
    int currentPathObj;
	string currentPathObjName;
    float distFromPath = 1;
    [SerializeField] float maxTorque = 100;
    float currentSpeed;
    [SerializeField] float topSpeed = 100;
    float decellarationSpeed = 30;
    Renderer breakingMesh;
    Material idleBreakLight;
    Material activeBreakLight;
    bool isBreaking;
	[SerializeField] float maxBreakTorque = 30;

    bool inSector;
    float sensorLength = 5;
    float frontSensorStartPoint = 5;
    float frontSensorSideDist = 5;
    float frontSensorsAngle = 30;
    float sidewaySensorLength = 5;
    float avoidSpeed = 10;
    private int flag = 0;

	float distToPoint = 0.0f;

	float AngleToPoint = 0.0f;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        GetPath();
		CalculateCurrentPathObj();
    }

	void CalculateCurrentPathObj()
	{
		Vector3 MyPosition = transform.position;
		int IdxOfClosePoint = 0;
		float minDistance = 10000000.0f;

		for (int i=0;i<path.Count ;++i)
		{
			var distance  = (MyPosition - path[i]).magnitude;
			if (distance < minDistance)
			{
				minDistance = distance;
				IdxOfClosePoint = i;
			}
		}

		currentPathObj = IdxOfClosePoint;
		

	}


    void GetPath()
    {


		var children = new List<Transform>();
		Transform[] root = pathGroup.GetComponentsInChildren<Transform>();
		foreach (Transform t in root)
		{
			if (t != null)
			{
				children.Add(t);
			}
		}
		path = children.OrderBy(t => t.gameObject.name).Select(t => t.position).ToList();
		path.Add(path[0]);

    }
    void Update()
    {
        if (flag == 0)
            GetSteer();
        Move();
        BreakingEffect();
        Sensors();
		ApplyBrakes(true);
    }

    void GetSteer()
    {
        Vector3 steerVector = transform.InverseTransformPoint(new Vector3(path[currentPathObj].x, transform.position.y, path[currentPathObj].z));
        float newSteer = maxSteer * (steerVector.x / steerVector.magnitude);
        wheelFL.steerAngle = newSteer;
        wheelFR.steerAngle = newSteer;
		distToPoint = steerVector.magnitude;
        if (steerVector.magnitude <= distFromPath)
        {
			//isBreaking = true;
			//ApplyBrakes(true);
            currentPathObj++;
            if (currentPathObj >= path.Count)
                currentPathObj = 0;
        }
		else
		{
			//isBreaking = false;
			//ApplyBrakes(false);
		}

    }

    void Move()
    {
        currentSpeed = 2 * (22 / 7) * wheelRL.radius * wheelRL.rpm * 60 / 1000;
        currentSpeed = Mathf.Round(currentSpeed);

		 var targetRotation = Quaternion.LookRotation(path[currentPathObj] - transform.position, Vector3.up);
         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 1.0f);   
		//transform.forward = Vector3.RotateTowards(transform.forward, path[currentPathObj] - transform.position, 5*Time.deltaTime, 0.0f);
         // move towards the target
         transform.position = Vector3.MoveTowards(transform.position, path[currentPathObj],   15*Time.deltaTime);

	
    }

	void ApplyBrakes(bool brake)
	{
		//Debug.Log (wheelFL.steerAngle);
		//if ((Mathf.Abs(wheelFL.steerAngle) > 20.0f) && currentSpeed > 30)
		AngleToPoint = Vector3.Angle(transform.position,path[currentPathObj+2]);
		//if (distToPoint < 20 && currentSpeed > 10)
		if (AngleToPoint > 25)
		{
		
		}
		else{
			isBreaking = false;
        
		}
		
	}
    void BreakingEffect()
    {
        // if (isBreaking)
        // {
        //     breakingMesh.material = activeBreakLight;
        // }
        // else
        // {
        //     breakingMesh.material = idleBreakLight;
        // }

    }

    void Sensors()
    {

        flag = 0;
        float avoidSenstivity = 0;
        Vector3 pos;
        RaycastHit hit;
        var rightAngle = Quaternion.AngleAxis(frontSensorsAngle, transform.up) * transform.forward;
        var leftAngle = Quaternion.AngleAxis(-frontSensorsAngle, transform.up) * transform.forward;



        pos = transform.position;
        pos += transform.forward * frontSensorStartPoint;

        //BRAKING SENSOR

        if (Physics.Raycast(pos, transform.forward, out hit, sensorLength))
        {
            if (hit.transform.tag != "Terrain")
            {
                flag++;
                //wheelRL.brakeTorque = decellarationSpeed;
                //wheelRR.brakeTorque = decellarationSpeed;
                Debug.DrawLine(pos, hit.point, Color.red);
            }
        }
        else
        {
            wheelRL.brakeTorque = 0;
            wheelRR.brakeTorque = 0;
        }

		#if _0_
        //Front Straight Right Sensor
        pos += transform.right * frontSensorSideDist;

        if (Physics.Raycast(pos, transform.forward, out hit, sensorLength))
        {
            if (hit.transform.tag != "Terrain")
            {
                flag++;
                avoidSenstivity -= 1;
                Debug.Log("Avoiding");
                Debug.DrawLine(pos, hit.point, Color.white);
            }
        }
        else if (Physics.Raycast(pos, rightAngle, out hit, sensorLength))
        {
            if (hit.transform.tag != "Terrain")
            {
                avoidSenstivity -= 0.5f;
                flag++;
                Debug.DrawLine(pos, hit.point, Color.white);
            }
        }


        //Front Straight left Sensor
        pos = transform.position;
        pos += transform.forward * frontSensorStartPoint;
        pos -= transform.right * frontSensorSideDist;

        if (Physics.Raycast(pos, transform.forward, out hit, sensorLength))
        {
            if (hit.transform.tag != "Terrain")
            {
                flag++;
                avoidSenstivity += 1;
                Debug.Log("Avoiding");
                Debug.DrawLine(pos, hit.point, Color.white);
            }
        }
        else if (Physics.Raycast(pos, leftAngle, out hit, sensorLength))
        {
            if (hit.transform.tag != "Terrain")
            {
                flag++;
                avoidSenstivity += 0.5f;
                Debug.DrawLine(pos, hit.point, Color.white);
            }
        }

        //Right SideWay Sensor
        if (Physics.Raycast(transform.position, transform.right, out hit, sidewaySensorLength))
        {
            if (hit.transform.tag != "Terrain")
            {
                flag++;
                avoidSenstivity -= 0.5f;
                Debug.DrawLine(transform.position, hit.point, Color.white);
            }
        }


        //Left SideWay Sensor
        if (Physics.Raycast(transform.position, -transform.right, out hit, sidewaySensorLength))
        {
            if (hit.transform.tag != "Terrain")
            {
                flag++;
                avoidSenstivity += 0.5f;
                Debug.DrawLine(transform.position, hit.point, Color.white);
            }
        }

        //Front Mid Sensor
        if (avoidSenstivity == 0)
        {

            if (Physics.Raycast(pos, transform.forward, out hit, sensorLength))
            {
                if (hit.transform.tag != "Terrain")
                {
                    if (hit.normal.x < 0)
                        avoidSenstivity = 1;
                    else
                        avoidSenstivity = -1;
                    Debug.DrawLine(pos, hit.point, Color.white);
                }
            }
        }
#endif
        //if (flag != 0)
        //    AvoidSteer(avoidSenstivity);


    }


    void AvoidSteer(float senstivity)
    {
        wheelFL.steerAngle = avoidSpeed * senstivity;
        wheelFR.steerAngle = avoidSpeed * senstivity;

    }
}
