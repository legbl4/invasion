using System;
using UnityEngine;
using System.Collections;
using Assets.Types;
using Assets.W.Scripts;
using Assets.W.Types;

public class PlasmaScript : Photon.MonoBehaviour
{
    public float maxLength = 25f;
    public float step = 10f;
    public float speed = 15f;

    public GameObject spawn;
    public GameObject explode; 


    private Vector3 shootForwardVector; 
    void Start()
    {
        if (GameManager.GameMode == GameMode.MultiPlayer && photonView.isMine == false) return; 
        shootForwardVector = spawn.transform.forward;
    }

    private PingInterpolation<Vector3> positionInterpolation = new PingInterpolation<Vector3>();
    private PingInterpolation<Quaternion> rotationInterpolation = new PingInterpolation<Quaternion>();
    private PingInterpolation<Vector3> scaleInterpolation = new PingInterpolation<Vector3>();
       
	void Update ()
	{
	    if (GameManager.GameMode == GameMode.MultiPlayer && photonView.isMine == false)
	    {
            //interpolate rendering only 
            if (GameManager.GameMode == GameMode.MultiPlayer)
                if (photonView.isMine == false)
                {
                    positionInterpolation.Interpolate((hostObject, smooth) =>
                    {
                        transform.position = Vector3.Lerp(transform.position, hostObject, smooth);
                    }, Time.deltaTime);

                    rotationInterpolation.Interpolate((hostObject, smooth) =>
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, hostObject, smooth);
                    }, Time.deltaTime);

                    scaleInterpolation.Interpolate((hostObject, smooth) =>
                    {
                        transform.localScale = Vector3.Lerp(transform.localScale, hostObject, smooth);
                    }, Time.deltaTime);
                    return;
                }

	    }

        if (transform.localScale.z < maxLength)
        {
            transform.localScale = new Vector3(transform.localScale.x, 
                transform.localScale.y , transform.localScale.z + step);
            
        }
	    transform.position = transform.position + shootForwardVector * speed;
	}

    //collisions here 
    void OnCollisionEnter(Collision collision)
    {
        if (GameManager.GameMode == GameMode.MultiPlayer)
            if (photonView.isMine == false)
                return;

        var contact = collision.contacts.Nearest(spawn.transform.position); 
        
        var marker = collision.gameObject.GetComponent<Markers>();
        if (marker == null)
        {
            GameManager.Manager.Destroy(gameObject, 0);
            return;
        }

        if (marker.IsTerrain)
            GameManager.Manager.Instantiate(explode, contact.point, Quaternion.identity);

        if (marker.IsRocket)
        {
            PhotonNetwork.player.SetScore(PhotonNetwork.player.GetScore() + Scores.DestroyRocket);
            var rocketOwner = collision.gameObject.GetComponent<PhotonView>().isMine;
            if (GameManager.GameMode == GameMode.MultiPlayer && rocketOwner == false)
            {
                var rocketView = collision.gameObject.GetComponent<PhotonView>();
                rocketView.RPC("DestroyRocket", PhotonTargets.MasterClient, rocketView.viewID);
            }
            else
            {
                //explosion 
                GameManager.Manager.Instantiate(explode, contact.point, Quaternion.identity);
                GameManager.Manager.Destroy(collision.gameObject, 0);
            }
        }

        if (marker.IsPlayerNetworkModel)
        {
            var playerView = collision.gameObject.GetComponent<PhotonView>();
            if (GameManager.GameMode == GameMode.MultiPlayer && playerView.isMine == false)
            {
                //RPC
                playerView.RPC("ExternalHit",
                    playerView.owner,
                    contact.point,
                    (int)Marker.IsShoot,
                    PhotonNetwork.player.ID);
            }
            else
            {
                collision.gameObject.GetComponent<PlayerInteraction>().Hit(contact.point, (int)Marker.IsShoot, PhotonNetwork.player.ID);
            }
        }
        GameManager.Manager.Destroy(gameObject, 0);
    }

    
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(transform.localScale);
            stream.SendNext(shootForwardVector);
        }
        else
        {
            var position = (Vector3) stream.ReceiveNext();
            var rotation = (Quaternion) stream.ReceiveNext();
            var scale = (Vector3) stream.ReceiveNext();

            positionInterpolation.Update(() => {
                transform.position = position;
            }, info, position);

            rotationInterpolation.Update(() => {
                transform.rotation = rotation;
            }, info, rotation);

            scaleInterpolation.Update(() =>
            {
                transform.localScale = scale;
            },info, scale);

            shootForwardVector = (Vector3) stream.ReceiveNext(); 
        }
    }

     
}

