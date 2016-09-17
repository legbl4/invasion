using UnityEngine;
using System.Collections;
using Assets.W.Types;

public class DestroyScript : Photon.MonoBehaviour
{

    public float DestroyTime = 3f;
    private float _currentTime; 
	void Start ()
	{
	    _currentTime = 0f; 
	}
	
	// Update is called once per frame
	void Update ()
	{
	    _currentTime += Time.deltaTime;
	    if (_currentTime >= DestroyTime)
	    {
            if(GameManager.GameMode == GameMode.MultiPlayer)
                if (photonView == null || photonView.isMine == false)
                    return;
            if(GameManager.GameMode == GameMode.MultiPlayer)
                if (photonView.isMine)
                {
                    var view = PhotonView.Find(photonView.viewID);
                    if (view == null)
                    {
                        Debug.Log("Destroy error. BUG" + photonView.name + ";" + photonView.viewID);
                        return; 
                    }
                    GameManager.Manager.Destroy(view.gameObject, 0);
                    return; 
                }
            GameManager.Manager.Destroy(gameObject, 0);
        }
	       
	}
}
