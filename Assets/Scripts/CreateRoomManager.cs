using System;
using System.Collections;
using Assets.W.Scripts;
using UnityEngine;
using Assets.W.Types;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class CreateRoomManager : Photon.PunBehaviour
{
    public NetworkStatusController StatusController; 
    public InputField RoomNameTextComponent;
    public InputField MaxPlayersTextComponent;
    public InputField NicknameTextComponent;
    public Dropdown GameDifficultDropdownComponent;


    private string _roomName; 
    void Start()
    {
        //TODO check with server
        RoomNameTextComponent.text = "room" + Random.Range(0, 100500);
        MaxPlayersTextComponent.text = 3.ToString();
        NicknameTextComponent.text = "player" + Random.Range(0, 100500);
        GameDifficultDropdownComponent.value = 1; 
    }

    public void OnCreateAndJoinButton()
    {
        var roomName = RoomNameTextComponent.text;
        if (roomName.Length <= 3)
        {
            StatusController.ForceUpdate("Room name to short");
            return;
        }
        
        //check player name
        var playerName = NicknameTextComponent.text;
        if (playerName == null || playerName.Length < 3)
        {
            StatusController.ForceUpdate("Invalid player name");
            return;
        }
        //check player count 
        var maxPlayersCount = 0; 
        try
        {
             maxPlayersCount = Convert.ToInt32(MaxPlayersTextComponent.text);
        }
        catch (Exception)
        {
            StatusController.ForceUpdate("Invalid max players count");
            return;
        }

        if (maxPlayersCount <= 1 || maxPlayersCount > 3)
        {
            StatusController.ForceUpdate("Invalid max players count. Must be (1; 3]");
            return;
        }

        var difficult = Difficult.Easy;
        switch (GameDifficultDropdownComponent.value)
        {
            case 1: 
                difficult = Difficult.Medium;
                break;
            case 2:
                difficult = Difficult.Hard;
                break;
            default:
                difficult = Difficult.Easy;
                break;
        }
        

        RoomOptions options = new RoomOptions()
        {
            maxPlayers = (byte)maxPlayersCount, 
            isVisible = true, 
            isOpen = true
        };

        //setup 
        GameManager.Difficult = difficult; 
        GameManager.GameMode = GameMode.MultiPlayer;
        GameManager.IsHost = true;
        GameManager.IsInitialized = false; 
        PhotonNetwork.player.name = playerName;

        var playerMarker = new Hashtable();
        playerMarker.Add("IsMaster", true);
        playerMarker.Add("Difficult", difficult);
        PhotonNetwork.player.SetCustomProperties(playerMarker);
        _roomName = roomName; 
        PhotonNetwork.CreateRoom(roomName, options, TypedLobby.Default);
        //PhotonNetwork.LeaveLobby(); 
    }

    public override void OnCreatedRoom()
    {
        SceneManager.LoadScene("Game");
        GameManager.CanStart = true; 
    }


    /*private void UpdateStatus(string message)
    {
        StatusObject.GetComponent<Text>().text = message; 
    }*/
}
