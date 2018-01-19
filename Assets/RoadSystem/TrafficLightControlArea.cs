using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrafficLightSide
{
    LL,
    RR,
    LR,
    RL
}
public class TrafficLightControlArea : MonoBehaviour
{

    [SerializeField] GSDRoadIntersection RoadIntersection;
    [SerializeField] TrafficLightSide TrafficSide;
    GSDTrafficLightController TrafficLightInfo;

    BoxCollider _BoxCollider;

    int vehicles = 0;
    // Use this for initialization
    void Start()
    {
        _BoxCollider = gameObject.GetComponent<BoxCollider>();
        switch (TrafficSide)
        {
            case TrafficLightSide.LL:
                TrafficLightInfo = RoadIntersection.LightsLL;
                break;
            case TrafficLightSide.RR:
                TrafficLightInfo = RoadIntersection.LightsRR;
                break;
            case TrafficLightSide.RL:
                TrafficLightInfo = RoadIntersection.LightsRL;
                break;
            case TrafficLightSide.LR:
                TrafficLightInfo = RoadIntersection.LightsLR;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

        GSDTrafficLightController.iLightStatusEnum myLightStatus = TrafficLightInfo.iLightStatus;
        
		/* Make sure no Vehicles inside the collider */
		if (vehicles == 0)
        {
            if (myLightStatus == GSDTrafficLightController.iLightStatusEnum.Red)
            {
                _BoxCollider.enabled = true;
                _BoxCollider.isTrigger = true;
            }
            else
            {
                _BoxCollider.enabled = false;
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        vehicles++;
    }


    void OnTriggerExit(Collider other)
    {
        vehicles--;
    }

    void OnTriggerStay(Collider other)
    {

    }

}
