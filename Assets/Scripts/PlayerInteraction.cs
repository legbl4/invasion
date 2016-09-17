using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Types;
using Assets.W.Scripts.DifficultSettings;
using Assets.W.Types;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class PlayerInteraction : Photon.MonoBehaviour
{

    public GameObject PlayerDestroyPrefab; 
    public GameObject PlayerHitPrefab;

    public GameObject[] DifficultPrefabs;
    //public GameObject UserHealthPointIndicator;
    public GameObject UserDamagedEffect;

    private int HealthPoint = 100;
    private bool isDamaged = false;


    private Settings _difficultSettings;

    void Start () {
        _difficultSettings = DifficultPrefabs[(int)GameManager.Difficult].GetComponent<Settings>();
    }

    public void Hit(Vector3 hitPoint, int marker, int attackingPlayerId)
    {
        Marker objectMarker = (Marker)marker;

        int damagedValue = objectMarker == Marker.IsShoot ?
           _difficultSettings.AnotherPlayerDamageByPlayer : _difficultSettings.AnotherPlayerDamageByRocket;

        //randomize
        damagedValue = Random.Range((int)(0.75 * damagedValue), damagedValue);
        HealthPoint = HealthPoint - damagedValue;

        var customPlayerProperties = PhotonNetwork.player.customProperties;
        customPlayerProperties["HP"] = HealthPoint; 
        PhotonNetwork.player.SetCustomProperties(customPlayerProperties); 
        
        GameManager.Manager.Instantiate(PlayerHitPrefab, hitPoint, Quaternion.identity);

        if (objectMarker == Marker.IsShoot)
        {
            //hit another player
            var attackingPlayer = PhotonNetwork.playerList.FirstOrDefault(player => player.ID == attackingPlayerId);
            if(attackingPlayer != null)
                attackingPlayer.SetScore(attackingPlayer.GetScore() + Scores.HitAnotherPlayer);
        }

        if (HealthPoint <= 40)
        {
            if(UserDamagedEffect.activeSelf ==false)
             UserDamagedEffect.SetActive(true);
        }
        if (HealthPoint <= 0)
        {
            if (objectMarker == Marker.IsShoot)
            {
                //hit another player
                var attackingPlayer = PhotonNetwork.playerList.FirstOrDefault(player => player.ID == attackingPlayerId);
                if (attackingPlayer != null)
                    attackingPlayer.SetScore(attackingPlayer.GetScore() + Scores.DestroyAnotherPlayer); 
            }
            //destroy effect
            GameManager.Manager.Instantiate(PlayerDestroyPrefab, transform.position, Quaternion.identity); 
            PhotonNetwork.Disconnect();
            GameManager.CanStart = false; 
            SceneManager.LoadScene("GameOver");
        }
    }

    [PunRPC]
    private void ExternalHit(Vector3 hitPoint, int marker, int attackingPlayerId)
    {
#if UNITY_ANDROID
         Handheld.Vibrate();
#endif
        Hit(hitPoint, marker, attackingPlayerId);
    }
}
