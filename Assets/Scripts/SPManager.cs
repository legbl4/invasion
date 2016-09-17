using System.Collections.Generic;
using System.Linq;
using Assets.W.Types;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.W.Scripts
{
    public class SPManager : GameObjectManager {

        public override object Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return Object.Instantiate(prefab, position, rotation);
        }

        public override void Destroy(GameObject prefab, float delay)
        {

            Object.Destroy(prefab, delay);
        }

        public override int GetId(GameObject gameObject)
        {
            return gameObject.GetInstanceID(); 
        }

        public override string ManagerName()
        {
            return "SPManager"; 
        }
    }
}
