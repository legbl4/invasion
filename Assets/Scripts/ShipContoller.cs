using System;
using UnityEngine;
using System.Collections;
using Assets.W.Scripts.DifficultSettings;
using Random = UnityEngine.Random;


public class ShipContoller : MonoBehaviour
{
    public Settings[] DifficultPrefabs;
    public GameObject[] ShipsPrefabs;
    public GameObject RocketPrefab;
    public GameObject ContainerControllerPrefab;

    private Settings _settings;

    private GameObject _containerControllerObject; 
    private ContainerController _containerController;

    public void ContainerInitialize()
    {
        _containerControllerObject = GameManager.Manager.Instantiate(ContainerControllerPrefab, Vector3.zero, Quaternion.identity) as GameObject; 
        _containerController = _containerControllerObject.GetComponent<ContainerController>();

        Action<GameObject> dropRocketAction = ship => AttackScript.Attack(ship.transform.position, RocketPrefab);
        //Action<GameObject> dropRocketAction = ship => Debug.Log("rocket"); 
        Action<GameObject> destroyShipAction = ship =>
        {
            //destroy damaged effect if exist 
            if (ship.GetComponent<ShipScript>().CurrentDamageEffect != null)
                GameManager.Manager.Destroy(ship.GetComponent<ShipScript>().CurrentDamageEffect, 0);
            GameManager.Manager.Destroy(ship, 0);
        };
        if(_settings == null)
            Start();
        _containerController.Initialize(dropRocketAction, destroyShipAction, _settings.ShipLiveTime, _settings.ContainerCount);
    }

    public void Reinitialize(GameObject containerControllerObject)
    {
        _containerControllerObject = containerControllerObject;
        _containerController = _containerControllerObject.GetComponent<ContainerController>();

        Action<GameObject> dropRocketAction = ship => AttackScript.Attack(ship.transform.position, RocketPrefab);
        Action<GameObject> destroyShipAction = ship =>
        {
            //destroy damaged effect if exist 
            if (ship.GetComponent<ShipScript>().CurrentDamageEffect != null)
                GameManager.Manager.Destroy(ship.GetComponent<ShipScript>().CurrentDamageEffect, 0);
            GameManager.Manager.Destroy(ship, 0);
        };

        _containerController.Reinitialize(dropRocketAction, destroyShipAction);
    }

	void Start ()
	{
        _settings = DifficultPrefabs[(int)GameManager.Difficult];
    }

    private float _currentTime = 0;
    private float _currentTimeBound = 0; 
	void Update ()
	{
	    _currentTime += Time.deltaTime; 
	    if (_currentTime >= _currentTimeBound)
	    {
	        _currentTime = 0;
	        _currentTimeBound = _settings.MinPeriod + Random.Range(_settings.MinPeriod, _settings.MaxPeriod) -
	                            _settings.MaxPeriod;
	        var containerId = _containerController.GetReadyContainerId();
	        if (containerId != -1)
	        {
	            var ship = GameManager.Manager.Instantiate(ShipsPrefabs[Random.Range(0, ShipsPrefabs.Length)], 
                    Vector3.zero, Quaternion.identity) as GameObject; 
                var shipId = _containerController.Attach(containerId, ship);
	            var shipScript = ship.GetComponent<ShipScript>(); 
                shipScript.ContainerController = _containerController;
	            shipScript.ContainerId = containerId;
	            shipScript.ShipId = shipId; 
	        }
	    }
	}
}
