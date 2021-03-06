using System;
using UnityEngine;
namespace Vehicles.Car
{
[RequireComponent(typeof (CarController))]
public class CarUserControl : MonoBehaviour
{
    private CarController m_Car; // the car controller we want to use


    private void Awake()
    {
        // get the car controller
        m_Car = GetComponent<CarController>();
    }


    private void FixedUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            return;
        }
        // pass the input to the car!
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
#if !MOBILE_INPUT
        float handbrake = Input.GetAxis("Jump");
        m_Car.Move(h, v, v, handbrake);
#else
            m_Car.Move(h, v, v, 0f);
#endif
    }
}
}
