using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthPointsIndicator : MonoBehaviour
{
    //public Text TextComponent;
    public Image PositiveImageComponent;
    public Image NegativeImageComponent;
    public float WidthCoef; 


    private IndicatorScript _currentIndicatorScript;
    private float _initialWidth; 
	// Use this for initialization
	void Start ()
	{
	    _currentIndicatorScript = gameObject.GetComponent<IndicatorScript>();
	    _initialWidth = PositiveImageComponent.rectTransform.sizeDelta.x; 
	}

    private int _prevHpCount = 100; 
	void Update ()
	{
	    var customProperties = PhotonNetwork.player.customProperties;
        int hp = 100;
	    if (customProperties.ContainsKey("HP"))
	        hp = (int)customProperties["HP"]; 
	    if (hp > 100 || hp < 0)
	        return;

	    if (hp == _prevHpCount)
	        return; 
        HpLineUpdate(hp);
	    _prevHpCount = hp; 
	}

    private void HpLineUpdate(int hp)
    {
        float percent = hp/100.0f;
        float currentWidth = percent*_initialWidth;
        float currentPos = (1 - percent)*WidthCoef; 
        PositiveImageComponent.transform.localPosition = new Vector3(
            currentPos, 
            PositiveImageComponent.transform.localPosition.y, 
            PositiveImageComponent.transform.localPosition.z);
        PositiveImageComponent.rectTransform.sizeDelta 
            = new Vector2(
                 currentWidth,
                 PositiveImageComponent.rectTransform.sizeDelta.y);
    }
}
