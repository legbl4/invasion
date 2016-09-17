using UnityEngine;
using System.Collections;

public class IndicatorReference : MonoBehaviour
{
    public IndicatorScript ScoreIndicator; 

	// Use this for initialization
	void Start () {
	    if(ScoreIndicator == null)
            throw new UnityException("ScoreIndicator not exist");
	}
}
