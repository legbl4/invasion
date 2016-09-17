using UnityEngine;
using System.Collections;

public class DynamicCameraScrpt : MonoBehaviour {

    public GameObject RotationCameraObject;
    public GameObject Player;

    public float MaxCameraConeRaduis;
    public float MaxCameraConeHeight;
    public float MinAngle = 0;
    public float MaxAngle = 90;

    private float MinAngleBoundary;
    private float MaxAngleBoundary;

    private int framesBeforeUpdate = 7;
    private int currentFrame;
    private float smooth = 7;
    private Vector3 targetPosition;
    void Start()
    {
        currentFrame = 0;
        MinAngle = MinAngle * Mathf.Deg2Rad;
        MaxAngle = MaxAngle * Mathf.Deg2Rad;
        UpdateCameraPosition();
        
    }

    void Update()
    {
        currentFrame++;
        if (currentFrame > framesBeforeUpdate)
        {
            UpdateCameraPosition();
            currentFrame = 0;
        }
        transform.position = Vector3.Lerp(transform.position, targetPosition, smooth * Time.deltaTime);
    }

    private void UpdateCameraPosition()
    {
        var currentAngle = RotationCameraObject.transform.localEulerAngles.x * Mathf.Deg2Rad;
        currentAngle = AngleNormalize(currentAngle);
        currentAngle = MinMaxCheck(currentAngle);
        var percent = (currentAngle - MinAngle) / (MaxAngle - MinAngle);
        var currentWidth = percent * MaxCameraConeRaduis;
        var currentHeighth = percent * MaxCameraConeHeight;
        this.targetPosition = new Vector3(
                -currentWidth * Mathf.Sin(RotationCameraObject.transform.localEulerAngles.y * Mathf.Deg2Rad) + Player.transform.position.x,
                currentHeighth + Player.transform.position.y,
                -currentWidth * Mathf.Cos(RotationCameraObject.transform.localEulerAngles.y * Mathf.Deg2Rad) + Player.transform.position.z
            );
    }

    private float MinMaxCheck(float angle)
    {
        var minBound = AngleNormalize(MinAngle - Mathf.PI / 2);
        var maxBound = AngleNormalize(MaxAngle + Mathf.PI / 2);
        if (AngleInRange(minBound, MinAngle, angle)) return MinAngle;
        if (AngleInRange(MaxAngle, maxBound, angle)) return MaxAngle;
        return angle;
    }

    private bool AngleInRange(float fromAngle, float toAngle, float targetAngle)
    {
        if (fromAngle > toAngle)
        {
            toAngle += 2 * Mathf.PI;
            if (targetAngle < fromAngle)
                targetAngle += 2 * Mathf.PI;
            if (targetAngle >= fromAngle && targetAngle <= toAngle)
                return true;
            return false;
        }
        return (targetAngle >= fromAngle && targetAngle <= toAngle);
    }

    private float AngleNormalize(float angle)
    {
        if (angle < 0)
        {
            while (angle < 0)
                angle += 2 * Mathf.PI;
            return angle;
        }
        if (angle >= 2 * Mathf.PI)
        {
            while (angle >= 2 * Mathf.PI)
                angle -= 2 * Mathf.PI;
            return angle;
        }
        return angle;
    }

    public GameObject GetCameraObject()
    {
        return RotationCameraObject;
    }
}

