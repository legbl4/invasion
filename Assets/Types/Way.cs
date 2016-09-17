using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ExitGames.Client.Photon;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Types
{ 
    class Way
    {
        public int Id; 
        public float Duration;
        public float EllapsedTime; 

        public GameObject AttachedObject;
        public bool CustomActionInvoked = false; 

        private Vector3[] _wayPoints;

        private WayScript _wayScript; 
        

        public Way()
        {
            _wayScript = new WayScript();
        }

        public void GenerateWayPoints(Transform containerTransform, float radius, float angle)
        {
            var points = new Vector3[4];
            //generate random StartPoint end end point 
            float currentAngle = Random.Range(containerTransform.eulerAngles.y, containerTransform.eulerAngles.y +  angle);
            points[0] = new Vector3(
                    containerTransform.position.x + radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad),
                    containerTransform.position.y,
                    containerTransform.position.z + radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad)
                );
            currentAngle = Random.Range(containerTransform.eulerAngles.y, containerTransform.eulerAngles.y + angle);
            points[1] = new Vector3(
                    containerTransform.position.x + (radius / 2) * Mathf.Sin(currentAngle * Mathf.Deg2Rad),
                    containerTransform.position.y,
                    containerTransform.position.z + (radius / 2) * Mathf.Cos(currentAngle * Mathf.Deg2Rad)
                );
            currentAngle = Random.Range(containerTransform.eulerAngles.y + 180, containerTransform.eulerAngles.y + 180 + angle);
            points[2] = new Vector3(
                    containerTransform.position.x + (radius / 2) * Mathf.Sin(currentAngle * Mathf.Deg2Rad),
                    containerTransform.position.y,
                    containerTransform.position.z + (radius / 2) * Mathf.Cos(currentAngle * Mathf.Deg2Rad)
                );
            currentAngle = Random.Range(containerTransform.eulerAngles.y + 180, containerTransform.eulerAngles.y + 180 + angle);
            points[3] = new Vector3(
                     containerTransform.position.x + radius * Mathf.Sin(currentAngle * Mathf.Deg2Rad),
                     containerTransform.position.y,
                     containerTransform.position.z + radius * Mathf.Cos(currentAngle * Mathf.Deg2Rad)
                 );
            _wayScript.points = points;
            _wayPoints = points; 
        }

        public bool Movement()
        {
            var deltaTime = EllapsedTime/Duration;
            if(EllapsedTime >= Duration)
                return false;
            var newPos = _wayScript.GetPoint(deltaTime);
            AttachedObject.transform.position = newPos; 
            return true;
        }

        public static byte[] OnSerialize(object target)
        {
            Way way = (Way) target;
            
            var idByteArray = BitConverter.GetBytes(way.Id);
            var durationByteArray = BitConverter.GetBytes(way.Duration);
            var ellapsedTimeByteArray = BitConverter.GetBytes(way.EllapsedTime);
            var customActionInvoked = BitConverter.GetBytes(way.CustomActionInvoked ? 1 : 0);
            var attachedObjectPhotonViewId = BitConverter.GetBytes(way.AttachedObject.GetComponent<PhotonView>().viewID);

            //serialize 
            List<byte[]> wayPointsArrays = new List<byte[]>();
            for (int i = 0; i < way._wayPoints.Length; i++)
            {
                Vector3 wayPoint = way._wayPoints[i];
               wayPointsArrays.Add(VectorToByte(wayPoint));
            }
            var resultIEnumerable = idByteArray
                .Concat(durationByteArray)
                .Concat(ellapsedTimeByteArray)
                .Concat(customActionInvoked)
                .Concat(attachedObjectPhotonViewId);
            resultIEnumerable = wayPointsArrays.Aggregate(resultIEnumerable, (current, t) => current.Concat(t));
            return resultIEnumerable.ToArray(); 
        }

        private static int VectorSize()
        {
            return VectorToByte(Vector3.zero).Length; 
        }

        private static byte[] VectorToByte(Vector3 vector)
        {
            var xByteArray = BitConverter.GetBytes(vector.x);
            var yByteArray = BitConverter.GetBytes(vector.y);
            var zByteArray = BitConverter.GetBytes(vector.z);
            return xByteArray.Concat(yByteArray).Concat(zByteArray).ToArray(); 
        }

        private static Vector3 BytesToVector(byte[] bytes)
        {
            List<float> coords = new List<float>();
            for (int i = 0; i < bytes.Length; i += 4)
            {
                byte[] current = new byte[4];
                for (int j = 0; j < 4; j++)
                    current[j] = bytes[i + j]; 
                coords.Add(BitConverter.ToSingle(current,0));
            }
            return new Vector3(coords[0],coords[1],coords[2]);
        }

        public static object OnDeserialize(byte[] bytes)
        {
            Way way = new Way();
            way._wayScript = new WayScript(); 

            way.Id = BitConverter.ToInt32(bytes, 0);
            way.Duration = BitConverter.ToSingle(bytes, 4);
            way.EllapsedTime = BitConverter.ToSingle(bytes, 8);
            way.CustomActionInvoked = BitConverter.ToInt32(bytes, 12) == 1;
            int viewId = BitConverter.ToInt32(bytes, 16);
            var obj = PhotonView.Find(viewId); 
            if(obj == null)
                throw  new UnityException("Cant find attached object on the way");

            way.AttachedObject = obj.gameObject; 

            List<Vector3> wayPoints = new List<Vector3>();
            for (int i = 20; i < bytes.Length; i += 12)
            {
                byte[] current = new byte[12];
                for (int j = 0; j < 12; j++)
                    current[j] = bytes[i + j]; 
                wayPoints.Add(BytesToVector(current));
            }
            way._wayPoints = wayPoints.ToArray();
            way._wayScript.points = wayPoints.ToArray(); 
            return way;
        }
    }
}
