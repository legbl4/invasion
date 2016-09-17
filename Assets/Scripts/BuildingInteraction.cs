using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Types;
using Assets.W.Scripts.DifficultSettings;
using Assets.W.Types;

public class BuildingInteraction : Photon.MonoBehaviour
{
    public GameObject HpLine;
    public GameObject BuildingDamagedPrefab;
    public GameObject BuildingDestroyPrefab;
    public GameObject BuildingHitEffect;

    public GameObject[] DifficultPrefabs;

    public GameObject FlamePoint; 
             
    private int HealthPoint = 100;
    private bool isDamaged = false;

    private GameObject buildingDamagedEffectObject; 

    private Settings _difficultSettings; 
    void Start()
    {
        _difficultSettings = DifficultPrefabs[(int)GameManager.Difficult].GetComponent<Settings>(); 
    }

    void OnCollisionEnter(Collision collision)
    {
        var component = collision.gameObject.GetComponent<Markers>();
        if (component == null) return; 

        if (component.IsShoot || component.IsRocket)
        {
            if (GameManager.GameMode == GameMode.SinglePlayer)
            {
                Hit(collision.contacts[0].point,
                    component.IsShoot ? (int)Marker.IsShoot : (int)Marker.IsRocket, PhotonNetwork.player.ID);
                return;
            }

            var buildingOwned = gameObject.GetComponent<PhotonView>().isMine;
            var collisionObjectOwned = collision.gameObject.GetComponent<PhotonView>().isMine;

            if (buildingOwned && collisionObjectOwned)
                Hit(collision.contacts[0].point,
                    component.IsShoot ? (int)Marker.IsShoot : (int)Marker.IsRocket, PhotonNetwork.player.ID);

            //ignore 
            if (buildingOwned && collisionObjectOwned == false)
                return;

            if (buildingOwned == false && collisionObjectOwned)
                if(component.IsShoot)
                    photonView.RPC("ExternalBuildingShoot", 
                        PhotonTargets.MasterClient, 
                        collision.contacts[0].point, 
                        component.IsShoot ? (int)Marker.IsShoot : (int)Marker.IsRocket, PhotonNetwork.player.ID);
            //ignore
            if (buildingOwned == false && collisionObjectOwned == false)
                return;
        }
    }

    [PunRPC]
    public void ExternalBuildingShoot(Vector3 hitPoint, int marker, int attackingPlayerId)
    {
        var buildingView = gameObject.GetComponent<PhotonView>();
        if (buildingView == null) return; 
        
        Hit(hitPoint, marker, attackingPlayerId);
    }

    private void Hit(Vector3 hitPoint, int marker, int attackingPlayerId)
    {
        Marker objectMarker = (Marker)marker;

        int damagedValue = objectMarker == Marker.IsShoot ? 
            _difficultSettings.BuildingDamageByPlayer : _difficultSettings.BuildingDamageByRocket;

        //randomize
        damagedValue = Random.Range((int)(0.75 * damagedValue), damagedValue);
        HealthPoint = HealthPoint - damagedValue; 

        var hpIndicator = HpLine.GetComponent<HpIndicator>();
        hpIndicator.SetHp = HealthPoint;

        if (objectMarker == Marker.IsShoot)
        {
            var attackingPlayer = PhotonNetwork.playerList.FirstOrDefault(player => player.ID == attackingPlayerId); 
            if(attackingPlayer == null)
                throw new UnityException("Attacing player not exist in playerList");
            attackingPlayer.SetScore(attackingPlayer.GetScore() + Scores.HitBuilding);
        }

        GameManager.Manager.Instantiate(BuildingHitEffect, hitPoint, Quaternion.identity); 
        //damae
        if (HealthPoint <= 50)
        {
            if (isDamaged == false)
            {
                isDamaged = true;
                buildingDamagedEffectObject = GameManager.Manager.Instantiate(BuildingDamagedPrefab,
                    FlamePoint.transform.position, Quaternion.identity) as GameObject;
                //buildingDamagedEffectObject.transform.parent = FlamePoint.transform; 
            }
        }

        //destroy 
        if (HealthPoint <= 0)
        {
            if (objectMarker == Marker.IsShoot)
            {
                var attackingPlayer = PhotonNetwork.playerList.FirstOrDefault(player => player.ID == attackingPlayerId);
                if (attackingPlayer == null)
                    throw new UnityException("Attacing player not exist in playerList");
                attackingPlayer.SetScore(attackingPlayer.GetScore() + Scores.DestroyBuilding);
            }

            GameManager.Manager.Destroy(gameObject,0);

            if (buildingDamagedEffectObject != null)
                GameManager.Manager.Destroy(buildingDamagedEffectObject,0);

            GameManager.Manager.Instantiate(BuildingDestroyPrefab, FlamePoint.transform.position, Quaternion.identity); 
        }
    }


    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(transform.localScale);
            stream.SendNext(HealthPoint);
            stream.SendNext(isDamaged);
            //send destroy effect viewId
            stream.SendNext(isDamaged ? buildingDamagedEffectObject.GetComponent<PhotonView>().viewID : 0);
        }
        else
        {
            transform.position = (Vector3) stream.ReceiveNext();
            transform.rotation = (Quaternion) stream.ReceiveNext();
            transform.localScale = (Vector3) stream.ReceiveNext();

            HealthPoint = (int) stream.ReceiveNext();
            HpLine.GetComponent<HpIndicator>().SetHp = HealthPoint;

            isDamaged = (bool) stream.ReceiveNext();
            var damagedEffectViewId = (int) stream.ReceiveNext();
            if (isDamaged)
            {
                buildingDamagedEffectObject = PhotonView.Find(damagedEffectViewId).gameObject;
            }

        }
    }
}
