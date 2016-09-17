using UnityEngine;
using System.Collections;

public class DEBUGCamera : MonoBehaviour
{
    public GameObject Camera;

    public int sensitivity = 2;
    public float minAngle = 85;
    public float maxAngle = 345; 


	// Update is called once per frame
	void Update () {
        var xAxis = -Input.GetAxis("Mouse Y");
        var yAxis = Input.GetAxis("Mouse X");

	    xAxis = transform.localEulerAngles.x + xAxis*sensitivity;
	    yAxis = transform.localEulerAngles.y + yAxis * sensitivity * 2;
	    if (xAxis < maxAngle && xAxis > minAngle) return; 
        transform.localEulerAngles = new Vector3(xAxis, yAxis, 0);
	}
}
