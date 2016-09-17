using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Types;
using Assets.W.Types;
using ExitGames.Client.Photon;
using Random = UnityEngine.Random;

public class Container : Photon.MonoBehaviour
{
    public float CaptureAngle;
    public float Radius;
    public Action<GameObject> EndAction;
    public Action<GameObject> CustomAction;

    [HideInInspector]
    public float MovementTime;

    [HideInInspector]
    public int ContainerId; 

    private List<Way> _currentWays = new List<Way>();

    /*void OnDrawGizmos()
    {
        if (CaptureAngle <= 0 || CaptureAngle >= 180)
        {
            Debug.Log("Invalid capture angle; (0;180) valid");
            return;
        }

        if (Radius <= 0)
        {
            Debug.Log("Invalid raduis");
            return;
        }

        GizmosDrawBounds();
        GizmosDrawContainer();
    }*/

    private void GizmosDrawBounds()
    {
        Color color = Color.green;
        var lerpSteps = CaptureAngle; 

        for (float angle = transform.eulerAngles.y ; angle < (transform.eulerAngles.y + CaptureAngle); angle += (CaptureAngle/lerpSteps))
        {
            Gizmos.color = color;
            var start = new Vector3(
                transform.position.x + Radius * Mathf.Sin(angle * Mathf.Deg2Rad),
                transform.position.y,
                transform.position.z + Radius * Mathf.Cos(angle * Mathf.Deg2Rad));
            var end = new Vector3(
                transform.position.x + Radius * Mathf.Sin((angle + 180) * Mathf.Deg2Rad),
                transform.position.y,
                transform.position.z + Radius * Mathf.Cos((angle + 180) * Mathf.Deg2Rad));
            Gizmos.DrawLine(start, end);
        }
        
    }

    private void GizmosDrawContainer()
    {
        Color containerBoundColor = Color.red;

        Gizmos.color = containerBoundColor; 
        float step = 10.0f;
        for (float currentAngle = 0; currentAngle <= 359; currentAngle += step)
        {
            var nextAngle = currentAngle + step; 
            var currentPoint = new Vector3(
                    transform.position.x + Radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad), 
                    transform.position.y, 
                    transform.position.z + Radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad)
                );
            var nextPoint = new Vector3(
                    transform.position.x + Radius * Mathf.Sin(nextAngle * Mathf.Deg2Rad),
                    transform.position.y,
                    transform.position.z + Radius * Mathf.Cos(nextAngle * Mathf.Deg2Rad)
                );
            Gizmos.DrawLine(currentPoint, nextPoint); 
        }
    }

	void Update ()
	{
	    if (GameManager.GameMode == GameMode.MultiPlayer && photonView.isMine == false)
	        return; 
        List<int> detachObjects = new List<int>();
	    foreach (var way in _currentWays)
	    {
	        var objectMoved = way.Movement();
	        way.EllapsedTime += Time.deltaTime;
	        if (objectMoved == false)
	        {
                detachObjects.Add(way.Id);
	            EndAction(way.AttachedObject); 
	        }
                
            //custim action(rocket) 
	        if (way.EllapsedTime >= MovementTime/2)
	        {
                if(way.CustomActionInvoked == false)
	            {
                    CustomAction(way.AttachedObject);
	                way.CustomActionInvoked = true; 
	            }
	        }
	    
	    }

	    foreach (var detachObjectId in detachObjects)
	        Detach(detachObjectId);
	}

    public int Attach(GameObject obj)
    {
        if(obj == null)
            throw new UnityException("Invalid object. Cant be null. Error in Attach method in Container script");
        //check collision; last way time > 30% generalTime 
        if(_currentWays.Exists(way => way.EllapsedTime < MovementTime * 0.3f))
            throw new UnityException("Cant attach object to this container now. Collisions protection.");

        var id = GetValidId();
        Way addedWay = new Way()
        {
            Id = id,
            EllapsedTime = 0, 
            Duration = MovementTime,
            AttachedObject = obj,
        };
        
        addedWay.GenerateWayPoints(transform, Radius, CaptureAngle);
        _currentWays.Add(addedWay);
        
        return id; 
    }

    public void Detach(int id)
    {
        var removedWay = _currentWays.FirstOrDefault(way => way.Id == id); 
        if(removedWay == null)
            throw new UnityException("Object with " + id  + " not exist in this container");
        _currentWays.Remove(removedWay); 
    }

    private int GetValidId()
    {
        for (int i = 0; i < Int32.MaxValue; i++)
        {
            if (_currentWays.Exists(way => way.Id == i) == false)
                return i; 
        }
        throw new UnityException("Size is overflow");
    }

    public bool IsReady()
    {
        //if there is a ship, who not reached the third of the way
        if (_currentWays.Exists(way => way.EllapsedTime <= MovementTime*0.3f))
            return false; 
        return true;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(this.CaptureAngle);
            stream.SendNext(this.ContainerId);
            stream.SendNext(this.MovementTime);
            stream.SendNext(this.Radius);
            stream.SendNext(_currentWays.Count);
            foreach (var currentWay in _currentWays)
                stream.SendNext(Way.OnSerialize(currentWay));
            
        }
        else
        {
            transform.position = (Vector3) stream.ReceiveNext();
            transform.rotation = (Quaternion) stream.ReceiveNext();
            this.CaptureAngle = (float) stream.ReceiveNext();
            this.ContainerId = (int) stream.ReceiveNext();
            this.MovementTime = (float) stream.ReceiveNext();
            this.Radius = (float) stream.ReceiveNext();
            int wayCount = (int) stream.ReceiveNext();
            _currentWays.Clear();
            for (int i = 0; i < wayCount; i++)
                _currentWays.Add((Way)Way.OnDeserialize((byte[])stream.ReceiveNext()));
            
        }
    }
}

