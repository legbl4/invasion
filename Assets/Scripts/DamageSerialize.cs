using UnityEngine;
using System.Collections;
using Assets.W.Types;

public class DamageSerialize : Photon.MonoBehaviour
{

    public int OwnerShipId = -1;

    private PingInterpolation<Vector3> positionInterpolation = new PingInterpolation<Vector3>();
    private PingInterpolation<Quaternion> rotationInterpolation = new PingInterpolation<Quaternion>();


    void Update()
    {
        if (GameManager.GameMode == GameMode.MultiPlayer && photonView.isMine == false)
        {

            positionInterpolation.Interpolate((hostObject, smooth) =>
            {
                transform.position = Vector3.Lerp(transform.position, hostObject, smooth);
            }, Time.deltaTime);
    
            rotationInterpolation.Interpolate((hostObject, smooth) =>
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, hostObject, smooth);
            }, Time.deltaTime);   
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(OwnerShipId);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            var ownerShipId = (int) stream.ReceiveNext(); 
            var position = (Vector3)stream.ReceiveNext();
            var rotation = (Quaternion)stream.ReceiveNext();

            OwnerShipId = ownerShipId; 

            positionInterpolation.Update(() => {
                transform.position = position;
            }, info, position);

            rotationInterpolation.Update(() => {
                transform.rotation = rotation;
            }, info, rotation);
        }
    }
}
