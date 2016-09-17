using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.W.Types;

public class HealthPointsController : MonoBehaviour
{
    public GameObject HealthPointsPrefab;

    private Dictionary<int, IndicatorScript> _playersIndicators = new Dictionary<int, IndicatorScript>();

    void Start () {
        if (HealthPointsPrefab == null)
            throw new UnityException("HP indicator prefab not exist");
        var currentPlayerProp = PhotonNetwork.player.customProperties;
        currentPlayerProp["HP"] = 100; 
        PhotonNetwork.player.SetCustomProperties(currentPlayerProp); 
    }


    private int prevHealthPoints = 100; 
	// Update is called once per frame
	void Update () {
	    var playerProp = PhotonNetwork.player.customProperties;
	    int healthPoints = 100;
	    if (playerProp.ContainsKey("HP"))
	        healthPoints = (int)playerProp["HP"];
	    Color currentColor;
	    if (healthPoints > 70)
	        currentColor = Color.green;
	    else if (healthPoints <= 70 && healthPoints >= 30)
	        currentColor = Color.yellow;
	    else currentColor = Color.red;

        if (GameManager.GameMode == GameMode.MultiPlayer)
        {
            //get score from another players 
            CheckUsers();
            var anotherPlayers = PhotonNetwork.otherPlayers;
            foreach (var anotherPlayer in anotherPlayers)
            {
                var anotherPlayerProp = anotherPlayer.customProperties;
                int hp = 100;
                if (anotherPlayerProp.ContainsKey("HP"))
                    hp = (int) anotherPlayerProp["HP"];
                UpdateHp(anotherPlayer, hp);
            }
        }
    }

    private void UpdateHp(PhotonPlayer player, int hp)
    {
        Color color; 
        if (hp > 70)
            color = Color.green;
        else if (hp <= 70 && hp >= 30)
            color = Color.yellow;
        else
            color = Color.red;

        //if indicator exist, just update
        if (_playersIndicators.ContainsKey(player.ID))
        {
            _playersIndicators[player.ID].UpdateText(hp.ToString(), color);
            return;
        }

        //else create 
        var playersModels = MarkersObjectsSearch.Search(new Marker[] { Marker.IsPlayerNetworkModel });
        var playerModel = playersModels.FirstOrDefault(pl => pl.GetComponent<PhotonView>().ownerId == player.ID);
        if (playerModel == null)
        {
            Debug.Log("Player exist, but not ready. Player network model not exist. PlayerId : " + player.ID + "; Name : " + player.name);
            return;
        }
        var hpIndicatorPosition = playerModel.GetComponent<IndicatorsPositionScript>().GetHealthPointsIndicatorPosition();
        var hpIndicator = Instantiate(HealthPointsPrefab, hpIndicatorPosition, Quaternion.identity) as GameObject;
        var currentPlayerModel =
            playersModels.FirstOrDefault(pl => pl.GetComponent<PhotonView>().ownerId == PhotonNetwork.player.ID);
        if (currentPlayerModel == null)
            throw new UnityException("Something is wrong :) Your player model not exist");
        hpIndicator.transform.LookAt(currentPlayerModel.transform.position);
        var indicatorScript = hpIndicator.GetComponent<IndicatorScript>();
        indicatorScript.UpdateText(hp.ToString(), color);
        _playersIndicators[player.ID] = indicatorScript;
    }

    private void CheckUsers()
    {
        List<int> toRemove = new List<int>();
        foreach (var playersIndicator in _playersIndicators)
        {
            //if player not exist in player list, but exist in dictionary
            if (PhotonNetwork.playerList.Any(player => player.ID == playersIndicator.Key) == false)
                toRemove.Add(playersIndicator.Key);
        }

        foreach (int id in toRemove)
        {
            Destroy(_playersIndicators[id].gameObject);
            _playersIndicators.Remove(id);
        }

    }
}
