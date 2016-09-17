using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.W.Types;

public class ScoreController : MonoBehaviour
{
    public GameObject ScorePrefab; 

    private Dictionary<int, IndicatorScript> _playersIndicators = new Dictionary<int, IndicatorScript>();
    private IndicatorScript _currentPlayerScoreComponent; 
	// Use this for initialization
	void Start ()
	{
        if(ScorePrefab == null)
            throw new UnityException("Score indicator prefab not exist");
        PhotonNetwork.player.SetScore(0);
    }
	
	// Update is called once per frame
	void Update () {
	    if (_currentPlayerScoreComponent == null)
	    {
	        var players = MarkersObjectsSearch.Search(new[] {Marker.IsPlayer})
                .Where(player => player.GetComponent<IndicatorReference>() != null).ToArray();
	        if (players.Length == 1)
	            _currentPlayerScoreComponent = players.First().GetComponent<IndicatorReference>().ScoreIndicator; 
	    }

        if (_currentPlayerScoreComponent != null)
            _currentPlayerScoreComponent.UpdateText(PhotonNetwork.player.GetScore().ToString(), new Color(0, 1, 0, 1));

        if (GameManager.GameMode == GameMode.MultiPlayer)
	    {
	        //get score from another players 
            CheckUsers();
	        var anotherPlayers = PhotonNetwork.otherPlayers;
	        foreach (var anotherPlayer in anotherPlayers)
	        {
	            var anotherPlayerScore = anotherPlayer.GetScore(); 
                UpdateScoreAnotherPlayer(anotherPlayer, anotherPlayerScore);
	        }
	    }
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

    
    private void UpdateScoreAnotherPlayer(PhotonPlayer player, int score)
    {
        //if indicator exist, just update
        if (_playersIndicators.ContainsKey(player.ID))
        {
            _playersIndicators[player.ID].UpdateText(score.ToString(), new Color(1, 0.7f, 0, 1));
            return; 
        }

        //else create 
        var playersModels = MarkersObjectsSearch.Search(new Marker[] {Marker.IsPlayerNetworkModel});
        var playerModel = playersModels.FirstOrDefault(pl => pl.GetComponent<PhotonView>().ownerId == player.ID);
        if (playerModel == null)
        {
            Debug.Log("Player exist, but not ready. Player network model not exist. PlayerId : " + player.ID + "; Name : " + player.name);
            return; 
        }
        var scoreIndicatorPosition = playerModel.GetComponent<IndicatorsPositionScript>().GetScoreIndicatorPosition();
        var scoreIndicator = Instantiate(ScorePrefab, scoreIndicatorPosition, Quaternion.identity) as GameObject;
        var currentPlayerModel =
            playersModels.FirstOrDefault(pl => pl.GetComponent<PhotonView>().ownerId == PhotonNetwork.player.ID); 
        if(currentPlayerModel == null)
            throw new UnityException("Something is wrong :) Your player model not exist");
        scoreIndicator.transform.LookAt(currentPlayerModel.transform.position);
        var indicatorScript = scoreIndicator.GetComponent<IndicatorScript>(); 
        indicatorScript.UpdateText(score.ToString(), new Color(1 ,0.7f, 0 ,1));
        _playersIndicators[player.ID] = indicatorScript; 
    }

   
}
