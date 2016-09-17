using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets;
using Assets.Types;
using Assets.W.Scripts;
using Assets.W.Scripts.DifficultSettings;
using Assets.W.Types;
using DigitalRuby.PyroParticles;
using ExitGames.Client.Photon;
using JetBrains.Annotations;
using Microsoft.Win32.SafeHandles;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Object = UnityEngine.Object;

public class GameManager : Photon.PunBehaviour
{
    public static GameMode GameMode = GameMode.SinglePlayer;
    public static Difficult Difficult = Difficult.Easy;
    public static bool IsInitialized = false;
    public static bool IsHost = false;
    public static bool CanStart = false;
    public static string RoomName; 

    public GameObject[] BuildingsSpawners;
    public GameObject[] BuildingPrefabs;
    public GameObject AimObject; 

    public GameObject PlayerPrefab;
    public GameObject[] PlayerSpawnersPrefabs;

    public GameObject[] DifficultPresets;

    public GameObject HostManagerObject;

    public GameObject RailGunPrefab; 

    private bool _alreadyInitialized = false;
    public static GameObjectManager Manager; 
    private GameObject DifficultInitialize()
    {
        GameObject difficult = DifficultPresets[(int)Difficult];
        return difficult;
    }

    private GameObjectManager ManagerInitialize()
    {
        GameObjectManager manager;
        switch (GameMode)
        {
            case GameMode.SinglePlayer:
                manager = new SPManager();
                break;
            default:
                manager = new MPManager();
                break;
        }
        return manager; 
    }

    void Update ()
    {
        if (CanStart == false) return; 
        if(_alreadyInitialized) return;

#if UNITY_ANDROID
        QualitySettings.SetQualityLevel(0);
#endif

        Application.targetFrameRate = 60; 
        _alreadyInitialized = true;
        DifficultInitialize();
        Manager = ManagerInitialize(); ; 
        if (IsHost == false)
        {
            CreatePlayer();
            return;
        }
	        
        if(IsInitialized)
            return;     
	    
        if (IsHost && IsInitialized == false)
	    {
            //if host and not initialized 
            HostManagerObject.SetActive(true);
            //ship controller initialize there 
            HostManagerObject.GetComponent<ShipContoller>().ContainerInitialize();
            BuildingInitialize();
            CreatePlayer();
        }
	    IsInitialized = true; 
	}

    private void SDK_OnBackButton()
    {
        if(GameMode == GameMode.MultiPlayer)
            PhotonNetwork.Disconnect();
        SceneManager.LoadScene("MainMenu");
    }

    private void CreatePlayer()
    {
        //search all exist players
        var players = MarkersObjectsSearch.Search(new [] {Assets.W.Types.Marker.IsPlayer} ); 

        //try find position 
        foreach (GameObject spawnerPrefab in PlayerSpawnersPrefabs)
        {
            if (players.Any(obj => obj.transform.position == spawnerPrefab.transform.position))
                continue;
            var player = Instantiate(PlayerPrefab,
                spawnerPrefab.transform.position,
                spawnerPrefab.transform.rotation) as GameObject;
            var railGun = GameManager.Manager.Instantiate(RailGunPrefab, spawnerPrefab.transform.position,
                spawnerPrefab.transform.localRotation) as GameObject;
            var cameraMarker = railGun.transform.Find("CameraMarker"); 
            player.GetComponent<DynamicCameraScrpt>().Player = cameraMarker.gameObject;
            var cameraHead = player.GetComponent<DynamicCameraScrpt>().GetCameraObject();
            railGun.GetComponent<PlayerInteraction>().UserDamagedEffect = cameraHead.transform.FindChild("ScreenDamage").gameObject;
            //AimObjects[0].GetComponent<Aiming>().Gun = railGun;
            railGun.GetComponent<RailgunMovement>().Camera = cameraHead;
            var leftGunSpawner = railGun.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).FindChild("LeftShootSpawn").gameObject;  
            var rightGunSpawner  = railGun.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).FindChild("RightShootSpawn").gameObject;
            AimObject.GetComponent<Aiming>().LeftGun = leftGunSpawner;
            AimObject.GetComponent<Aiming>().RigthGun = rightGunSpawner;
#if UNITY_ANDROID || UNITY_IOS
            Cardboard.SDK.OnBackButton -= SDK_OnBackButton;
            Cardboard.SDK.OnBackButton += SDK_OnBackButton;
#endif
            return;
        }
    }

    private void BuildingInitialize()
    {
        if (BuildingsSpawners.Length != BuildingPrefabs.Length)
            throw new UnityException("Invalid building spawners count or building prefabs count");

        for(int i = 0; i < BuildingPrefabs.Length; i++)
        {
            var building = GameManager.Manager.Instantiate(BuildingPrefabs[i],
                BuildingsSpawners[i].transform.position,
                BuildingPrefabs[i].transform.rotation) as GameObject;
            building.transform.localScale = BuildingPrefabs[i].transform.localScale;
        }
    }

    /// <summary>
    /// change host, when master disconnect 
    /// </summary>
    /// <param name="newMasterClient"></param>
    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        if (PhotonNetwork.player.ID != newMasterClient.ID) return;

        GameManager.IsHost = true;
        var gameObjects = MarkersObjectsSearch.Search(
            new[]
            {
                Marker.IsBuilding,
                Marker.IsShip,
                Marker.IsShipSpawner,
                Marker.IsShipTarget,
                Marker.IsNetworkCommunicableEffect,
                Marker.IsRocket,
                Marker.IsContainer,
                Marker.IsContainerController
            }); 
        foreach (GameObject obj in gameObjects)
        {
            var view = obj.GetComponent<PhotonView>();

            var lastOwner = PhotonNetwork.playerList.FirstOrDefault( (player) =>
            {
                if (player.customProperties == null) return false;
                object isMaster = false;  
                if(player.customProperties.TryGetValue("IsMaster", out isMaster) == false) return false;
                return (bool)isMaster; 
            });
            if (lastOwner == null)
                throw new UnityException("Cant find last master player"); 

            if (view.ownerId == lastOwner.ID)
                view.RequestOwnership();
        }

        var gameSettings = new Hashtable();
        gameSettings.Add("IsMaster", true);
        gameSettings.Add("Difficult", Difficult);
        newMasterClient.SetCustomProperties(gameSettings); 

        //hosts scripts 
        var difficult = DifficultInitialize();
        var manager = ManagerInitialize();
        HostManagerObject.SetActive(true);

        var containerController = MarkersObjectsSearch.Search(new[] {Marker.IsContainerController}); 
        if(containerController.Length != 1)
            throw  new UnityException("Cant find Container controller. Method OnMasterClientSwitch");
        HostManagerObject.GetComponent<ShipContoller>().Reinitialize(containerController.First());
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        if (PhotonNetwork.player.isMasterClient == false) return;
        var gameObjects = MarkersObjectsSearch.Search(new[]
        {
            Marker.IsPlayerNetworkModel,
            Marker.IsShoot
        });
        foreach (var o in gameObjects)
        {
            var view = o.GetComponent<PhotonView>(); 
            if(view.ownerId == otherPlayer.ID)
                PhotonNetwork.Destroy(view);
        }
    }

    public override void OnOwnershipRequest(object[] viewAndPlayer)
    {
        PhotonView view = viewAndPlayer[0] as PhotonView;
        PhotonPlayer requestingPlayer = viewAndPlayer[1] as PhotonPlayer;

        if (requestingPlayer.isMasterClient)
            view.TransferOwnership(requestingPlayer);
        
    }

    void OnLevelWasLoaded(int level)
    {
        if (GameMode == GameMode.SinglePlayer) CanStart = true; 
        if(GameMode == GameMode.MultiPlayer && IsHost == false)
            if (level == SceneManager.GetSceneByName("Game").buildIndex)
                PhotonNetwork.JoinRoom(RoomName); 
        
    }

    public override void OnJoinedRoom()
    {
        if (IsHost)
        {
            CanStart = true;
            return; 
        }
        var masterPlayer = PhotonNetwork.masterClient; 
        object difficult;
        masterPlayer.customProperties.TryGetValue("Difficult", out difficult);
        Difficult = (Difficult)difficult;
        CanStart = true;
    }
}

public enum PresetTypes
{
    Building = 0, 
    Ships = 1
}

