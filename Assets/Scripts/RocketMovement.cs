using UnityEngine;
using System.Collections;
using Assets.W.Scripts.DifficultSettings;
using Assets.W.Types;

public class RocketMovement : Photon.MonoBehaviour
{
    public Vector3 Target;
    public GameObject[] DifficultPresets;
    public GameObject Explode;
    private Settings _difficult; 

	void Start ()
	{
	    _difficult = DifficultPresets[(int) GameManager.Difficult].GetComponent<Settings>(); 
	}
	

    private PingInterpolation<Vector3> _positionInterpolation = new PingInterpolation<Vector3>();

	void Update ()
	{
	    if (GameManager.GameMode == GameMode.MultiPlayer && photonView.isMine == false)
	    {
            //interpolate
            _positionInterpolation.Interpolate(((position, smooth) =>
            transform.position = Vector3.Lerp(transform.position, position,smooth)), Time.deltaTime);
	        return;
	    }
        
	    transform.position = Vector3.MoveTowards(transform.position, Target, _difficult.RocketSpeed);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(Target);
        }
        else
        {
            var position = (Vector3) stream.ReceiveNext(); 
            _positionInterpolation.Update(() => transform.position = position,
                info, 
                position);
            transform.rotation = (Quaternion) stream.ReceiveNext();
            Target = (Vector3) stream.ReceiveNext(); 
        }
    }

    [PunRPC]
    public void DestroyRocket(int viewId)
    {
        Debug.Log("RPC recieve. View id : " + viewId);
        var view = PhotonView.Find(viewId);
        if (view == null) return;
        GameManager.Manager.Instantiate(Explode, view.transform.position, Quaternion.identity);
        GameManager.Manager.Destroy(view.gameObject, 0);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (GameManager.GameMode == GameMode.MultiPlayer)
            if (photonView.isMine == false)
            {
                return;
            }


        var contact = collision.contacts[0];
        var marker = collision.gameObject.GetComponent<Markers>();
        if (marker == null)
        {
            GameManager.Manager.Destroy(gameObject, 0);
            return;
        }
        //rocket destroying managed by plasma
        if (marker.IsShoot)
            return; 
        if (marker.IsTerrain)
            GameManager.Manager.Instantiate(Explode, contact.point, Quaternion.identity);
        if (marker.IsShip)
            return;
        //player destroy rocket 
        if (marker.IsPlayerNetworkModel)
        {
            var playerView = collision.gameObject.GetComponent<PhotonView>();
            if (GameManager.GameMode == GameMode.MultiPlayer && playerView.isMine == false)
            {
                playerView.RPC("ExternalHit",
                    playerView.owner,
                    contact.point,
                    (int)Marker.IsRocket, -1);
            }
            else
            {
                collision.gameObject.GetComponent<PlayerInteraction>().Hit(contact.point, (int)Marker.IsRocket, -1);
            }
        }
        GameManager.Manager.Destroy(gameObject, 0);
    }

}
