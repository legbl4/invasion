using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.W.Types;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MPRoomManager : MonoBehaviour
{
    public Text NickName;
    public NetworkStatusController StatusController;
    public SelectedManager SelectManager; 

    public void OnCreateRoom()
   {
        if (PhotonNetwork.connected == false)
        {
            StatusController.ForceUpdate("Connecton failed");
            return;
        }
       SceneManager.LoadScene("CreateRoom");
   }

    public void OnJoinGame()
    {
        if (NickName.text.Length < 3)
        {
            StatusController.ForceUpdate("Invalid nickname.");
            return;
        }

        if (SelectManager.Selected == null)
        {
            StatusController.ForceUpdate("No selected rooms");
            return;
        }
        var roomName = SelectManager.Selected.GetComponent<RoomInfoComponentScript>().GetRoomName();
        var roomList = PhotonNetwork.GetRoomList();
        if (roomList.Any(room => room.name == roomName) == false)
        {
            StatusController.ForceUpdate("Room not exist");
            return;
        }
        var currentRoom = roomList.First(room => room.name == roomName);
        if (currentRoom.playerCount == currentRoom.maxPlayers)
        {
            StatusController.ForceUpdate("Room is full");
            return;
        }

        GameManager.GameMode = GameMode.MultiPlayer;
        GameManager.IsHost = false;
        GameManager.IsInitialized = false;

        GameManager.RoomName = roomName; 
        SceneManager.LoadScene("Game");
     }
}
