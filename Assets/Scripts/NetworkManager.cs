using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using Assets.W.Types;
using ExitGames.Client.Photon;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Random = UnityEngine.Random;

public class NetworkManager : Photon.PunBehaviour
{
    public string GameVersion = "v1.0";

    public event Action<string> OnStatusChanged;
    public event Action<RoomInfo[]> OnRoomsInfoChanged;

    private bool _readyToConnect = false;
    private bool _initialized = false;  
	void Update ()
	{
	    if (_readyToConnect && _initialized == false)
	    {
            if (PhotonNetwork.connected)
                PhotonNetwork.Disconnect();
            var connected = PhotonNetwork.ConnectUsingSettings(GameVersion);
            if (connected == false)
            {
                if(OnStatusChanged != null)
                    OnStatusChanged("Cant connect to server");
                return;
            }
            if (OnStatusChanged != null)
                OnStatusChanged("Connected success");
            _initialized = true; 
	    }
	}

    public void SignOnStatusChangeEvent(Action<string> action)
    {
        _readyToConnect = true; 
        OnStatusChanged += action; 
    }

    public void SignOnRoomsInfoChangeEvent(Action<RoomInfo[]> action)
    {
        OnRoomsInfoChanged += action; 
    }

   

    public override void OnConnectedToMaster()
    {
        OnStatusChanged("Connected to master"); 
        PhotonNetwork.autoCleanUpPlayerObjects = false; 
        PhotonNetwork.JoinLobby(TypedLobby.Default); 
    }

    public override void OnJoinedLobby()
    {
        OnStatusChanged("Joined to lobby");
    }

    public override void OnReceivedRoomListUpdate()
    {
        var rooms = PhotonNetwork.GetRoomList();
        if(OnRoomsInfoChanged != null)
            OnRoomsInfoChanged(rooms); 
        /*var fakeRooms = new RoomInfo[25];
        for (int i = 0; i < fakeRooms.Length; i++)
            fakeRooms[i] = new RoomInfo("Room " + i, new Hashtable());
        
        if (OnRoomsInfoChanged != null)
            OnRoomsInfoChanged(fakeRooms);*/
    }

    
}
