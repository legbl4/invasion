using System;
using UnityEngine;
using System.Collections;

public class RoomsController : MonoBehaviour
{
    public NetworkManager Manager;
    public GameObject RoomInfoPrefab;
    public GameObject RoomsContainer;
    public RectTransform ContainerControlPoint; 

	void Start () {
	    Manager.SignOnRoomsInfoChangeEvent(RoomInfoChangedHandler); 
	}

    private void RoomInfoChangedHandler(RoomInfo[] roomInfoArray)
    {
        var roomInfoPrefabRectTransform = RoomInfoPrefab.GetComponent<RectTransform>();
        for (int i = 0; i < RoomsContainer.transform.childCount; i++)
        {
            var child = RoomsContainer.transform.GetChild(i).gameObject; 
            if(child.name != "ControlPoint")
                Destroy(child);
        }
            
        
        for (int i = 0; i < roomInfoArray.Length; i++)
        {
            var roomInfoLabel = GameObject.Instantiate(RoomInfoPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            var roomInfoComponentScript = roomInfoLabel.GetComponent<RoomInfoComponentScript>();
            roomInfoComponentScript.Parrent = RoomsContainer;
            roomInfoComponentScript.SetPlayerCount(roomInfoArray[i].playerCount, roomInfoArray[i].maxPlayers);
            roomInfoComponentScript.SetRoomName(roomInfoArray[i].name);
            roomInfoComponentScript.Manager = GetComponent<SelectedManager>(); 
            roomInfoLabel.transform.SetParent(RoomsContainer.transform);
            roomInfoLabel.transform.localScale = new Vector3(1,1,1);
            
           roomInfoLabel.transform.localPosition
                = new Vector3(0, ContainerControlPoint.localPosition.y - (i * roomInfoLabel.GetComponent<RectTransform>().sizeDelta.y), 0);
        }
    }
}
