using UnityEngine;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;

public class Aiming : MonoBehaviour
{
    public GameObject LeftGun;
    public GameObject RigthGun; 

    public float maxDistance = 400f;
    public float maxScale = 3f;

    //update every 25 frames and lerp
    private float _calcTime = 1f/60f;
    private float _frameTime = 0;
    private Vector3 _target = Vector3.zero; 
    
    void Update()
    {
        _frameTime += Time.deltaTime;
        transform.position = Vector3.Lerp(transform.position, _target, Time.deltaTime / _calcTime);
        //if (_frameTime < _calcTime) return;
        

        //else update aim
        _frameTime = 0; 
        if (LeftGun == null || RigthGun == null) return;
        //control point at middle 
        Vector3 controlPoint = Vector3.Lerp(LeftGun.transform.position, RigthGun.transform.position, 0.5f); 
        transform.rotation = LeftGun.transform.rotation;

        Ray ray = new Ray(controlPoint, transform.forward);

        //raycast all for left aim and not plasma and min distanse
        var nearestLeftObject = Physics.RaycastAll(ray, 500f)
            .Where(raycast =>
                raycast.transform.gameObject.GetComponent<Markers>() == null ||
                raycast.transform.gameObject.GetComponent<Markers>().IsShoot == false
            ).OrderBy(raycast => raycast.distance).FirstOrDefault();


        if (nearestLeftObject.Equals(default(RaycastHit)) == false)
        {
            var leftLength = maxDistance;
            if (nearestLeftObject.distance < maxDistance)
            {
                leftLength = nearestLeftObject.distance - 0.5f;
                var leftLocalScale = maxScale*(leftLength/maxDistance);
                transform.localScale = new Vector3(leftLocalScale, leftLocalScale, leftLocalScale);
            }
            else
            {
                transform.localScale = new Vector3(maxScale, maxScale, maxScale);
            }

            //if (_target == Vector3.zero) _target = transform.position;
            //else
                _target = new Vector3(
                    controlPoint.x + transform.forward.x*leftLength,
                    controlPoint.y + transform.forward.y*leftLength,
                    controlPoint.z + transform.forward.z*leftLength); 
            /*transform.position = new Vector3(
                controlPoint.x + transform.forward.x*leftLength,
                controlPoint.y + transform.forward.y*leftLength,
                controlPoint.z + transform.forward.z*leftLength);*/
        }
        else
        {
            transform.localScale = new Vector3(maxScale, maxScale, maxScale);

            //if (_target == Vector3.zero) _target = transform.position;
            //else
                _target = new Vector3(
                    controlPoint.x + transform.forward.x*maxDistance,
                    controlPoint.y + transform.forward.y*maxDistance,
                    controlPoint.z + transform.forward.z*maxDistance); 
            /*transform.position = new Vector3(
                controlPoint.x + transform.forward.x * maxDistance,
                controlPoint.y + transform.forward.y * maxDistance,
                controlPoint.z + transform.forward.z * maxDistance);*/
        }
    }
}
