using System;
using UnityEngine;
using System.Collections;
using Assets.W.Types;
using UnityEngine.UI;

public class GameoverSceneController : MonoBehaviour
{
    public GameObject RestartButton;
    public Text TextComponent; 

    public int RestartDalayTime = 10; 


	void Start () {
	    if (GameManager.GameMode == GameMode.SinglePlayer)
	    {
	        RestartButton.SetActive(true);
	        TextComponent.text = "Restart(" + RestartDalayTime + ")"; 
	    }
	}

    private float _currentTime = 0;
    private float _prevTime = 0;  
	void Update () {
	    if (GameManager.GameMode == GameMode.SinglePlayer)
	    {
	        _prevTime = _currentTime; 
	        _currentTime += Time.deltaTime;
	        if (Convert.ToInt32(_prevTime) < Convert.ToInt32(_currentTime))
                TextComponent.text = "Restart(" + (RestartDalayTime - Convert.ToInt32(_currentTime)) + ")";
	        if (_currentTime >= RestartDalayTime)
	            RestartButton.GetComponent<MenuScript>().OnSingleRestart();
	    }
	}
}
