using Assets.W.Types;
using UnityEngine;

namespace Assets.W.Scripts
{
    public class MPManager : GameObjectManager {
        public override object Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            Debug.Log("Try instante object " + prefab.name);
            return PhotonNetwork.Instantiate(prefab.name, position, rotation, 0); 
        }


        public override void Destroy(GameObject gameObject, float delay)
        {
            var photonView = gameObject.GetComponent<PhotonView>();
            var viewId = photonView == null ? 0 : photonView.viewID; 
            Debug.Log("Try destroy object " + gameObject.name + ". ViewId  : " + viewId);
            var photonViewObject = PhotonView.Find(viewId); 
            if(photonViewObject != null)
                PhotonNetwork.Destroy(gameObject); 
        }

        public override int GetId(GameObject gameObject)
        {
            var view = gameObject.GetComponent<PhotonView>();
            return view.viewID;
        }

        public override string ManagerName()
        {
            return "MPManager"; 
        }
    }
}
