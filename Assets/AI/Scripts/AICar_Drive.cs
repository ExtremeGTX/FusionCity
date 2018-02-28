using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum BrakeMode
{
    None,
    Sensor,
    DistToWaypoint
}
public class AICar_Drive : MonoBehaviour
{
    public Vector3 centerOfMass;
    //List<Transform> path;
    List<Transform> path;
    [SerializeField] Transform pathGroup;
    [SerializeField] bool ReversePath;
    float maxSteer = 35.0f;
    [SerializeField] WheelCollider wheelFL;
    [SerializeField] WheelCollider wheelFR;
    [SerializeField] WheelCollider wheelRL;
    [SerializeField] WheelCollider wheelRR;

    List<WheelCollider> m_WheelColliders;
    int currentPathObj;
    string currentPathObjName;
    float distFromPath = 4;
    [SerializeField] float maxTorque = 100;
    float currentSpeed;
    [SerializeField] float topSpeed = 100;
    float decellarationSpeed = 50;
    bool DistBrake;
    bool SensorBrake;

    BrakeMode ActiveBrake;
    [SerializeField] float maxBreakTorque = 30;

    float distToPoint = 0.0f;

    float AngleToPoint;

    string waypointName;

    string hitObject;
    float SensorDistToObject = 0.0f;
    void Start()
    {
       m_WheelColliders = new List<WheelCollider>();
        m_WheelColliders.Add(wheelFL);
        m_WheelColliders.Add(wheelFR);
        m_WheelColliders.Add(wheelRL);
        m_WheelColliders.Add(wheelRR);

        GetComponent<Rigidbody>().centerOfMass = centerOfMass;
        GetPath();
        CalculateCurrentPathObj();

        
    }

    void CalculateCurrentPathObj()
    {
        Vector3 MyPosition = transform.position;
        int IdxOfClosePoint = 0;
        float minDistance = 10000000.0f;

        for (int i = 0; i < path.Count; ++i)
        {
            var distance = (MyPosition - path[i].position).magnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                IdxOfClosePoint = i;
            }
        }

        currentPathObj = IdxOfClosePoint;
        waypointName = path[currentPathObj].name;

    }


    void GetPath()
    {


        var children = new List<Transform>();
        Transform[] root = pathGroup.GetComponentsInChildren<Transform>();
        foreach (Transform t in root)
        {
            if (t != pathGroup)
            {
                children.Add(t);
            }
        }
        //path = children.OrderBy(t => t.gameObject.name).Select(t => t.position).ToList();

            path = children.OrderBy(t => t.gameObject.name).Select(t => t).ToList();

        
        //path.Add(path[0]);

    }
    void Update()
    {
        GetSteer();
        CalculateSpeed();
        //        BreakingEffect();

        wheelRL.motorTorque = 0;
        wheelRR.motorTorque = 0;



        float distToObject = Sensors();
        if (distToObject != 0.0f)
        {
            if (distToObject < 5)
            {
                wheelRL.brakeTorque = 10000.0f;
                wheelRR.brakeTorque = 10000.0f;
            }
            else
            {
                wheelRL.brakeTorque = maxTorque * 5;
                wheelRR.brakeTorque = maxTorque * 5;
            }
        }
        else if (CheckForTurns() > 40 && currentSpeed > 25)
        {
            DistBrake = true;
            wheelRL.brakeTorque = maxTorque * 3;
            wheelRR.brakeTorque = maxTorque * 3;
        }

        else
        {
            DistBrake = false;
            wheelRL.motorTorque = maxTorque;
            wheelRR.motorTorque = maxTorque;
            wheelRL.brakeTorque = 0.0f;
            wheelRR.brakeTorque = 0.0f;
        }

    }

    void GetSteer()
    {
        
        Vector3 steerVector = transform.InverseTransformPoint(new Vector3(path[currentPathObj].position.x, transform.position.y, path[currentPathObj].position.z));
        float newSteer = maxSteer * (steerVector.x / steerVector.magnitude);

        // var rotation = Quaternion.LookRotation(path[currentPathObj].position - transform.position);
        // newSteer = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 6.0f).y;
        

        wheelFL.steerAngle = newSteer;
        wheelFR.steerAngle = newSteer;
        distToPoint = steerVector.magnitude;

        if (steerVector.magnitude <= distFromPath)
        {
            if (ReversePath)
            {
                currentPathObj--;
                if (currentPathObj < 0)
                currentPathObj = path.Count-1;

            }
            else
            {
                currentPathObj++;
                if (currentPathObj >= path.Count)
                currentPathObj = 0;

            }
            
            
            waypointName = path[currentPathObj].name;
        }

    }

    void CalculateSpeed()
    {
        /* Speed in m/s */
        currentSpeed = 2 * (22 / 7) * wheelRL.radius * wheelRL.rpm * 60 / 1000;
        currentSpeed = Mathf.Round(currentSpeed);
        currentSpeed = (gameObject.GetComponent<Rigidbody>().velocity.magnitude*3600)/1000;


        // for (int i = 0; i < 4; i++)
        // {
        //     Quaternion quat;
        //     Vector3 position;
        //     m_WheelColliders[i].GetWorldPose(out position, out quat);
        //     m_WheelColliders[i].GetComponentInParent<Transform>().position = position;
        //     m_WheelColliders[i].GetComponentInParent<Transform>().rotation = quat;
        // }

    }

    float CheckForTurns()
    {
        int IdxCheckpoint = currentPathObj + 2 > (path.Count - 1) ? 1 : currentPathObj + 2;
        Vector3 relative = transform.InverseTransformPoint(path[IdxCheckpoint].position);
        AngleToPoint = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
        return Mathf.Abs(AngleToPoint);
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

    float Sensors()
    {

        Vector3 pos;
        RaycastHit hit;


        float SensorLength = currentSpeed < 5 ? 5 : currentSpeed;
        pos = transform.position;
        pos += transform.forward;
        pos.y += 0.5f;

        //BRAKING SENSOR

        if (Physics.Raycast(pos, transform.forward, out hit, SensorLength))
        {
            Debug.DrawLine(pos, hit.point, Color.red);
            SensorDistToObject = hit.distance;
            hitObject = hit.transform.name;
            SensorBrake = true;
            ActiveBrake = BrakeMode.Sensor;
            return hit.distance;
        }

        SensorBrake = false;
        return 0.0f;
    }

}
