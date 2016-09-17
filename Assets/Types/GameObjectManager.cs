using UnityEngine;

namespace Assets.W.Types
{
    public abstract class GameObjectManager
    {
        public abstract object Instantiate(GameObject prefab, Vector3 position, Quaternion rotation);

        public abstract void Destroy(GameObject prefab, float delay);

        public abstract int GetId(GameObject gameObject); 
        public abstract string ManagerName();
    }
}
