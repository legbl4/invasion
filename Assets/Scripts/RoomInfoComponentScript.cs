using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RoomInfoComponentScript : MonoBehaviour
{
    public Text RoomNameTextComponent;
    public Text PlayersCountTextComponent;
    public float ComponentPadding = 0.1f;
    public float TextPadding = 0.05f; 
    

    [HideInInspector] public GameObject Parrent;

    [HideInInspector] public SelectedManager Manager;

    [HideInInspector] public bool Selected = false;

    void Start()
    {
        if(Parrent == null)
            throw new UnityException("RoomInfoComponentScript throw exception. Parrent gameobject not exist");
        if(RoomNameTextComponent == null)
            throw new UnityException("RoomInfoComponentScript throw exception. RoomNameTextComponent Text not exist");
        if (PlayersCountTextComponent == null)
            throw new UnityException("RoomInfoComponentScript throw exception. PlayersCountTextComponent Text not exist");

        var transformRect = GetComponent<RectTransform>();
        var parrentTransformRect = Parrent.GetComponent<RectTransform>(); 
        transformRect.sizeDelta = new Vector2(parrentTransformRect.sizeDelta.x - parrentTransformRect.sizeDelta.x * (ComponentPadding / 2.0f), 
            transformRect.sizeDelta.y);
        RoomNameTextComponent.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(transformRect.sizeDelta.x - transformRect.sizeDelta.x * TextPadding, transformRect.sizeDelta.y);
        PlayersCountTextComponent.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(transformRect.sizeDelta.x - transformRect.sizeDelta.x * TextPadding, transformRect.sizeDelta.y);
    }

    public void OnClick()
    {
        Manager.Selected = gameObject; 
    }

    public void SetRoomName(string roomName)
    {
        RoomNameTextComponent.text = roomName; 
    }

    public string GetRoomName()
    {
        return RoomNameTextComponent.text;
    }

    public void SetPlayerCount(int current, int global)
    {
        PlayersCountTextComponent.text = current + " / " + global; 
    }
}
