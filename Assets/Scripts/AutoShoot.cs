using UnityEngine;
using System.Collections;
using Assets.W.Scripts;
using Assets.W.Types;

public class AutoShoot : Photon.MonoBehaviour
{
    public float period = 100f;

    public GameObject leftShootSpawn;
    public GameObject rightShootSpawn;
    public GameObject shootPrefab;
    public GameObject ShootEffect; 

    private float leftCurrentFrame;
    private float rigthCurrentFrame;
    
	void Start ()
	{
	    leftCurrentFrame = 0;
	    rigthCurrentFrame = period/2;
	}
	
	// Update is called once per frame
	void Update ()
	{
        if(GameManager.GameMode == GameMode.MultiPlayer)
            if (photonView.isMine == false)
                return; 
	    leftCurrentFrame++;
	    rigthCurrentFrame++;
	    if (leftCurrentFrame >= period)
	    {
	        leftCurrentFrame = 0;
            var shoot = GameManager.Manager.Instantiate(shootPrefab, leftShootSpawn.transform.position,
                leftShootSpawn.transform.rotation) as GameObject;
            shoot.GetComponent<PlasmaScript>().spawn = leftShootSpawn;
            //
	        var shootEffect = GameManager.Manager.Instantiate(ShootEffect, leftShootSpawn.transform.position,
	            Quaternion.identity) as GameObject;
	    }
	    if (rigthCurrentFrame >= period)
	    {
	        rigthCurrentFrame = 0;
            var shoot  = GameManager.Manager.Instantiate(shootPrefab, rightShootSpawn.transform.position,
                rightShootSpawn.transform.rotation) as GameObject;
	        shoot.GetComponent<PlasmaScript>().spawn = rightShootSpawn;

	        var shootEffect = GameManager.Manager.Instantiate(ShootEffect, rightShootSpawn.transform.position,
	            Quaternion.identity); 
	    }
	}
}
