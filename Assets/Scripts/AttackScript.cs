using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.W.Types;


public static class AttackScript
{
    public static void Attack(Vector3 dropPoint, GameObject RocketPrefab)
    {
        //only master can manage attack 
        if (GameManager.GameMode == GameMode.MultiPlayer && PhotonNetwork.player.isMasterClient == false) return;

         //can drop rocket 
         var objectTarget = RocketTarget();
         if (objectTarget == null)
         {
             Debug.Log("ERROR. Cant find any attacked object");
             return; 
         }
         var rocket = GameManager.Manager.Instantiate(RocketPrefab, dropPoint, Quaternion.identity) as GameObject;
         rocket.transform.LookAt(objectTarget.transform.position);
         rocket.GetComponent<RocketMovement>().Target = objectTarget.transform.position; 
    }

    private static GameObject RocketTarget()
    {
        //try find buildings 
        var buildings = MarkersObjectsSearch.Search(new[] {Marker.IsBuilding});
        if (buildings.Length > 0)
        {
            //attack building 
            return buildings[Random.Range(0, buildings.Length)]; 
        }
        else
        {
            //attack player
            var players = MarkersObjectsSearch.Search(new[] {Marker.IsPlayerNetworkModel});
            if (players.Length == 0) return null;
            return players[Random.Range(0, players.Length)]; 
        }
    }

}
