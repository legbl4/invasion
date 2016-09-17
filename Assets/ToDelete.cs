using UnityEngine;
using System.Collections;

public class ToDelete : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    var child = transform.GetChild(0).gameObject;
	    child.GetComponent<RoomInfoComponentScript>().Parrent = gameObject; 
	}
}
