using System;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100.0f;
    public float clampAngle = 80.0f;

    private float rotY = 0.0f; // rotation around the up/y axis
    private float rotX = 0.0f; // rotation around the right/x axis

    void Start()
    {
        Vector3 rot = transform.localEulerAngles;
        rotY = rot.y;
        rotX = rot.x;
    }

    void Update()
    {
        if (!Input.GetMouseButton(1))
        {
            return;
        }
		
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = -Input.GetAxis("Mouse Y");

        rotY += mouseX * mouseSensitivity * 0.1f;
        rotX += mouseY * mouseSensitivity * 0.1f;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;

    	Vector3 pos = Camera.main.transform.localPosition;
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // forward
        {
			if (Input.GetKey(KeyCode.LeftShift))
			{
				pos.z = pos.z +1;					
			}
			else
			{
				pos.y = pos.y -1;
			}
			
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // back
        {
			if (Input.GetKey(KeyCode.LeftShift))
			{
				pos.z = pos.z -1;					
			}
			else
			{
				pos.y = pos.y +1;
			}
        }
		Camera.main.transform.localPosition = pos;
    }
}