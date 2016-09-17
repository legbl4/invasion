using UnityEngine;
using System.Collections;

public class IndicatorsPositionScript : MonoBehaviour
{
    public GameObject ScoreIndicatorContainer;
    public GameObject HealthPointsIndicatorContainer; 
	// Use this for initialization
	void Start () {
	    if(ScoreIndicatorContainer == null)
            throw  new UnityException("Score indicator container is missed");
        if(HealthPointsIndicatorContainer == null)
            throw new UnityException("Health Points indicator container is missed");
	}

    public Vector3 GetScoreIndicatorPosition()
    {
        return ScoreIndicatorContainer.transform.position; 
    }

    public Vector3 GetHealthPointsIndicatorPosition()
    {
        return HealthPointsIndicatorContainer.transform.position; 
    }
}
