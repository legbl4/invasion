using UnityEngine;
using System.Collections;

public class DynamicRotation : MonoBehaviour
{
    private Vector3 PrevPosition; 
	// Use this for initialization
	void Start ()
	{
	    PrevPosition = transform.position; 
	}
	
	// Update is called once per frame
	void Update ()
	{
	    var currentPosition = transform.position;
	    transform.position = PrevPosition; 
        transform.LookAt(currentPosition);
	    transform.position = currentPosition; 
	    PrevPosition = currentPosition; 
	}
}
