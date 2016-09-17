using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.W.Types;
using UnityEngine.Experimental.Director;
using Random = UnityEngine.Random;

public class ContainerController : Photon.MonoBehaviour
{
    public GameObject ContainerPrefab;
    public float MinHeigth = 400;

    [HideInInspector]
    public int ContainersCount;

    private Container[] _containers; 

    public void Initialize(Action<GameObject> customAction, Action<GameObject> endAction, float duration, int containersCount)
    {
        ContainersCount = containersCount; 
        _containers = new Container[ContainersCount];
        for (int i = 0; i < ContainersCount; i++)
        {
            var containerPosition = new Vector3(
                    transform.position.x,
                    transform.position.y + MinHeigth + 40 * i,
                    transform.position.z
                );
            var container = GameManager.Manager.Instantiate(ContainerPrefab, containerPosition, Quaternion.identity) as GameObject;
            container.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
            container.GetComponent<Container>().ContainerId = (i + 1);
            container.GetComponent<Container>().CustomAction = customAction;
            container.GetComponent<Container>().EndAction = endAction;
            container.GetComponent<Container>().MovementTime = duration;
            _containers[i] = container.GetComponent<Container>(); 
        }
    }

    public void Reinitialize(Action<GameObject> customAction, Action<GameObject> endAction)
    {
        foreach (var container in _containers)
        {
            container.CustomAction = customAction;
            container.EndAction = endAction; 
        }
    }

    public int GetReadyContainerId()
    {
        List<int> valiablieIds = new List<int>();
        for (int i = 0; i < _containers.Length; i++)
        {
            var containerScript = _containers[i].GetComponent<Container>(); 
            if(containerScript.IsReady())
                valiablieIds.Add(containerScript.ContainerId);
        }
        if (valiablieIds.Count == 0)
            return -1;
        return valiablieIds[Random.Range(0, valiablieIds.Count)]; 
    }

    public int Attach(int containerId, GameObject obj)
    {
        var container = _containers.FirstOrDefault(cont => cont.GetComponent<Container>().ContainerId == containerId); 
        if(container == null)
            throw  new UnityException("Invalid container id. Method attach, id " + containerId);
        return container.GetComponent<Container>().Attach(obj); 
    }

    public void Detach(int containerId, int objectId)
    {
        var container = _containers.FirstOrDefault(cont => cont.GetComponent<Container>().ContainerId == containerId);
        if(container == null) 
            throw new UnityException("Container with " + containerId + " id not exist");
        container.GetComponent<Container>().Detach(objectId);
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(MinHeigth);
            stream.SendNext(ContainersCount);
            //reestablish _containers array 
            for (int i = 0; i < ContainersCount; i++)
            {
                var containerScript = _containers[i];
                var containerObject = containerScript.gameObject;
                stream.SendNext(containerObject.GetComponent<PhotonView>().viewID);
            }
        }
        else
        {
            this.MinHeigth = (float) stream.ReceiveNext();
            this.ContainersCount = (int) stream.ReceiveNext();
            if (_containers == null)
            {
                _containers = new Container[ContainersCount];

                var allExistContainersObjects = MarkersObjectsSearch.Search(new[] {Marker.IsContainer}); 
                for (int i = 0; i < ContainersCount; i++)
                {
                    var viewId = (int) stream.ReceiveNext();
                    var containerObject =
                        allExistContainersObjects.FirstOrDefault(
                            cont => cont.GetComponent<PhotonView>().viewID == viewId); 
                    if(containerObject == null)
                        throw new UnityException("Container with + " + viewId + " not exist in object hierarhy");
                    _containers[i] = containerObject.GetComponent<Container>(); 
                }
            }
        }
    }

}
