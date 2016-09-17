using System;
using System.Linq;
using Assets;
using Assets.W.Scripts.DifficultSettings;
using Assets.W.Types;

using UnityEngine;
using Marker = Assets.W.Types.Marker;


public class ShipSpawnScript : MonoBehaviour
    {
        public GameObject DifficultSetting;
        public GameObject[] ShipsPrefabs;
        public GameObjectManager Manager;
        public GameObject ShipSpawnerPrefab;
        public GameObject ShipTargetPrefab;

        //tags 
        public string ShipSpawnerTag;
        public string ShipTargetTag;

        private TrajectorySpawner[] _spawners;
        private Settings _difficult;


 
        void Start()
        {
            var spawners = MarkersObjectsSearch.Search(new[] { Assets.W.Types.Marker.IsShipSpawner});
            var targets = MarkersObjectsSearch.Search(new []{ Assets.W.Types.Marker.IsShipTarget }); 
           
            _difficult = DifficultSetting.GetComponent<Settings>();
            _spawners = new TrajectorySpawner[spawners.Length];

            for (int i = 0; i < spawners.Length; i++)
            {
                var currentSpawner = spawners[i];
                var currentTarget = targets.First(t => Math.Abs(t.transform.position.x - currentSpawner.transform.position.x) < 0.1f);
                _spawners[i] = new TrajectorySpawner()
                {
                    StartPoint = currentSpawner.transform,
                    EndPoint = currentTarget.transform,
                    CurrentFrame = 0,
                    CurrentUpperBoundFrameLimit = UnityEngine.Random.Range(_difficult.MinPeriod, _difficult.MaxPeriod)
                };
            }

        }

        void Update()
        {
            for (int i = 0; i < _spawners.Length; i++)
            {
                _spawners[i].Update();
                if (_spawners[i].CurrentFrame >= _spawners[i].CurrentUpperBoundFrameLimit)
                {
                    var spawner = _spawners[i].StartPoint; 

                    var ship = Manager.Instantiate(
                        ShipsPrefabs[UnityEngine.Random.Range(0, ShipsPrefabs.Length)],
                        spawner.position,
                        spawner.rotation
                        ) as GameObject;

                    var shipMovementScript = ship.GetComponent<ShipScript>();
                    //shipMovementScript.Trajectory = _spawners[i]; 
                    _spawners[i].Drop();
                    _spawners[i].UpperBoundUpdate(_difficult);
                }
            }
        }
    }

    public struct TrajectorySpawner
    {
        public Transform StartPoint;
        public Transform EndPoint;
        public float CurrentUpperBoundFrameLimit;
        public float CurrentFrame;

        public void Update()
        {
            CurrentFrame = CurrentFrame + 1;
        }

        public void Drop()
        {
            CurrentFrame = 0;
        }

        public void UpperBoundUpdate(Settings difficult)
        {
            CurrentUpperBoundFrameLimit = UnityEngine.Random.Range(difficult.MinPeriod, difficult.MaxPeriod);
        }
    }

