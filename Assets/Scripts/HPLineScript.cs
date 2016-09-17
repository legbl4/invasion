using UnityEngine;
using System.Collections;

public class HPLineScript : MonoBehaviour
{
    public GameObject Positive;
    public GameObject Negative;
    

    private int _hpCount;
    public int SetHp
    {
        set
        {
            if (value < 0) return;
            _hpCount = value;
            UpdateStatus();
        }
    }

	void Start ()
	{
	    _hpCount = 100; 
	}

    private void UpdateStatus()
    {
        var generalSize =  Negative.transform.localScale;
        float percent = _hpCount / 100.0f; 
        Positive.transform.localScale = new Vector3(
                generalSize.x * percent,
                Positive.transform.localScale.y, 
                Positive.transform.localScale.z
            );
    }
}
