using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets;
using Assets.Types;
using Assets.W.Scripts.DifficultSettings;
using Assets.W.Types;
using UnityEngine.SocialPlatforms.Impl;

public class ShipScript : Photon.MonoBehaviour {

    public GameObject ShipExplosionPrefab;
    public GameObject ShipDamagedEffectPrefab;
    public GameObject ShipHitPrefab;
    public GameObject FlameSpawnObject;

    public GameObject[] DifficultPresets;

    [HideInInspector]
    public GameObject CurrentDamageEffect;

    [HideInInspector]
    public ContainerController ContainerController;

    [HideInInspector]
    public int ContainerId;

    [HideInInspector]
    public int ShipId; 

    private int hitCount = 0;
    private bool _isDamaged = false;

    private Settings _difficult;

    private PingInterpolation<Vector3> positionInterpolation = new PingInterpolation<Vector3>();
    private PingInterpolation<Quaternion> rotationInterpolation = new PingInterpolation<Quaternion>();


    void Start()
    {
        _difficult = DifficultPresets[(int)GameManager.Difficult].GetComponent<Settings>();
    }

    void Update()
    {
        if (GameManager.GameMode == GameMode.MultiPlayer)
            if (photonView.isMine == false)
            {
                positionInterpolation.Interpolate((hostObj, smooth) =>
                {
                    transform.position = Vector3.Lerp(transform.position, hostObj, smooth);
                }, Time.deltaTime);

                rotationInterpolation.Interpolate((hostObj, smooth) =>
                {
                    transform.rotation = Quaternion.Lerp(transform.rotation, hostObj, smooth);
                }, Time.deltaTime);
            }   
        //destroy damage effect 
    }



    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(hitCount);
            //send controller info 
            stream.SendNext(ContainerController.GetComponent<PhotonView>().viewID);
            stream.SendNext(ContainerId);
            stream.SendNext(ShipId);

            stream.SendNext(_isDamaged);
            stream.SendNext(_isDamaged ? CurrentDamageEffect.GetComponent<PhotonView>().viewID : 0);
        }
        else
        {
            var position = (Vector3)stream.ReceiveNext();
            var rotation = (Quaternion)stream.ReceiveNext();

            positionInterpolation.Update(() => {
                transform.position = position;
            }, info, position);

            rotationInterpolation.Update(() =>
            {
                transform.rotation = rotation;
            }, info, rotation);

            hitCount = (int)stream.ReceiveNext();
            int containerContainerViewID = (int) stream.ReceiveNext();
            if (ContainerController == null)
            {
                var containerView = PhotonView.Find(containerContainerViewID); 
                if(containerView == null)
                    throw new UnityException("Container controller not exist");
                ContainerController = containerView.gameObject.GetComponent<ContainerController>(); 
            }

            ContainerId = (int) stream.ReceiveNext();
            ShipId = (int) stream.ReceiveNext();
            _isDamaged = (bool)stream.ReceiveNext();
            var damagedEffectViewId = (int)stream.ReceiveNext();
            if (_isDamaged && CurrentDamageEffect == null)
            {
                var photonViewObject = PhotonView.Find(damagedEffectViewId);
                if(photonViewObject == null)
                    throw new UnityException("BUG BUG BUG");
                CurrentDamageEffect = photonViewObject.gameObject;
                CurrentDamageEffect.transform.parent = transform; 
            }
        }
    }

    private void Hit(Vector3 hitPoint, int attackingPlayerId)
    {
        hitCount++;
        var shipHit = GameManager.Manager.Instantiate(ShipHitPrefab, hitPoint, Quaternion.identity);

        var attackingPlayer = PhotonNetwork.playerList.FirstOrDefault(player => player.ID == attackingPlayerId);
        if (attackingPlayer != null)
            attackingPlayer.SetScore(attackingPlayer.GetScore() + Scores.HitShip);

        if (hitCount >= _difficult.ShipMaxHit / 2 && _isDamaged == false)
        {
            CurrentDamageEffect = GameManager.Manager.Instantiate(ShipDamagedEffectPrefab,
                FlameSpawnObject.transform.position, Quaternion.identity) as GameObject;
            CurrentDamageEffect.transform.parent = FlameSpawnObject.transform;
            if (GameManager.GameMode == GameMode.MultiPlayer)
                CurrentDamageEffect.GetComponent<DamageSerialize>().OwnerShipId = photonView.viewID; 
            
            _isDamaged = true;
        }

        if (hitCount >= _difficult.ShipMaxHit)
        {
            if (attackingPlayer != null)
                attackingPlayer.SetScore(attackingPlayer.GetScore() + Scores.DestroyShip);

            GameManager.Manager.Instantiate(ShipExplosionPrefab, transform.position, Quaternion.identity);
            if (CurrentDamageEffect != null)
                GameManager.Manager.Destroy(CurrentDamageEffect, 0);
            //detach ship from container and remove
            ContainerController.Detach(ContainerId, ShipId);
            GameManager.Manager.Destroy(gameObject, 0);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        var component = collision.gameObject.GetComponent<Markers>();
        if (component == null) return;

        if (component.IsShoot)
        {
            if (GameManager.GameMode == GameMode.SinglePlayer)
            {
                Hit(collision.contacts[0].point, PhotonNetwork.player.ID);
                return;
            }

            var shipOwned = gameObject.GetComponent<PhotonView>().isMine;
            var collisionObjectOwned = collision.gameObject.GetComponent<PhotonView>().isMine;

            if (shipOwned && collisionObjectOwned)
                Hit(collision.contacts[0].point, PhotonNetwork.player.ID);

            //ignore 
            if (shipOwned && collisionObjectOwned == false)
                return;

            if (shipOwned == false && collisionObjectOwned)
                photonView.RPC("ExternalHit", PhotonTargets.MasterClient, collision.contacts[0].point, PhotonNetwork.player.ID);
            //ignore
            if (shipOwned == false && collisionObjectOwned == false)
                return;
        }
    }

    [PunRPC]
    public void ExternalHit(Vector3 hitPoint, int attackingPlayerId)
    {
        var shipView = gameObject.GetComponent<PhotonView>();
        if (shipView == null) return;

        Hit(hitPoint, attackingPlayerId);

    }
}
