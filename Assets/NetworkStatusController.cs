using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class NetworkStatusController : MonoBehaviour
{
    public NetworkManager Manager;
    public float DelayTime = 3.0f; 


    private string _status = "";
    private float _currentTime = 0;
    private bool _isActive = false;


    private GUIStyle _style;
    void OnGUI()
    {
        if (_style == null)
        {
            _style = new GUIStyle();
            _style.normal.textColor = Color.white;
            _style.fontSize = 28;
        }
        GUI.Label(new Rect(0, 0, Screen.width , Screen.height), _status, _style); 
    }

	void Start () {
	    Manager.SignOnStatusChangeEvent((message) =>
	    {
	        _status = message;
	        _isActive = true; 
	    });
	}

    public void ForceUpdate(string text)
    {
        _status = text;
        _isActive = true;
    }
	
	// Update is called once per frame
	void Update () {
	    if (_isActive)
	    {
	        _currentTime += Time.deltaTime;
	        if (_currentTime >= DelayTime)
	        {
	            _isActive = false;
	            _currentTime = 0; 
	            _status = ""; 
	        }
	    }
	}
}
