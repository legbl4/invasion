
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.W.Scripts; 
namespace Assets.W.Types
{
    public static class MarkersObjectsSearch {
        public static GameObject[] Search(Marker[] markers)
        {
            var allObjects = Object.FindObjectsOfType<GameObject>();
            List<GameObject> result = new List<GameObject>();
            foreach (var gameObject in allObjects)
            {
                var markerScript = gameObject.GetComponent<Markers>(); 
                if(markerScript == false) continue;
                if (markers.Any( marker => ContainsMarker(markerScript, marker)))
                     result.Add(gameObject); 
            }
            return result.ToArray(); 
        }

        private static bool ContainsMarker(Markers markerScript, Marker requiredMarker)
        {
            if(requiredMarker == Marker.IsBuilding && markerScript.IsBuilding)
                return true;
            if (requiredMarker == Marker.IsPlayer && markerScript.IsPlayer)
                return true; 
            if(requiredMarker == Marker.IsPlayerNetworkModel && markerScript.IsPlayerNetworkModel)
                return true;
            if (requiredMarker == Marker.IsShip && markerScript.IsShip)
                return true;
            if (requiredMarker == Marker.IsTurrel && markerScript.IsTurrel)
                return true; 
            if(requiredMarker == Marker.IsShoot && markerScript.IsShoot)
                return true;
            if(requiredMarker == Marker.IsTerrain && markerScript.IsTerrain)
                return true;
            if (requiredMarker == Marker.IsRocket && markerScript.IsRocket)
                return true; 
            if(requiredMarker == Marker.IsNetworkCommunicableEffect && markerScript.IsNetworkCommunicableEffect)
                return true;
            if(requiredMarker == Marker.IsContainer && markerScript.IsContainer)
                return true;
            if(requiredMarker == Marker.IsContainerController && markerScript.IsContainerController)
                return true;
            return false; 
        }
    }
}