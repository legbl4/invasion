using System;
using UnityEngine;
using System.Collections;
using Assets.W.Types;

public class RailgunMovement : Photon.MonoBehaviour
{
    public GameObject Camera;
    public GameObject RailgunRotationPart;
    public GameObject LeftGun;
    public GameObject RightGun;

    private float rotationSmooth = 10f; 
    

    void Start()
    {
        nextRotation = RotationUpdate();
        _currentFrame = 0; 
    }


    private Vector3 nextRotation; 
    private int _currentFrame;
    private int _dropFramesCount = 10;  

    private PingInterpolation<Quaternion> rotationPartInterpolation = new PingInterpolation<Quaternion>();
    private PingInterpolation<Quaternion> leftGunInterpolation = new PingInterpolation<Quaternion>();
    private PingInterpolation<Quaternion> rightGunInterpolation = new PingInterpolation<Quaternion>();
    void Update()
    {
        //interpolate rendering only 
        if (GameManager.GameMode == GameMode.MultiPlayer)
            if (photonView.isMine == false)
            {
                rotationPartInterpolation.Interpolate((hostObj, smooth) =>
                {
                    RailgunRotationPart.transform.rotation = Quaternion.Lerp(
                        RailgunRotationPart.transform.rotation, hostObj, smooth);
                }, Time.deltaTime);

                leftGunInterpolation.Interpolate((hostObj, smooth) =>
                {
                    LeftGun.transform.rotation = Quaternion.Lerp(
                        LeftGun.transform.rotation, hostObj, smooth);
                }, Time.deltaTime);

                rightGunInterpolation.Interpolate((hostObj, smooth) =>
                {
                    RightGun.transform.rotation = Quaternion.Lerp(
                        RightGun.transform.rotation, hostObj, smooth);
                }, Time.deltaTime);
                return;
            }

        _currentFrame++;
        if (_currentFrame >= _dropFramesCount)
        {
            _currentFrame = 0;
            nextRotation = RotationUpdate(); 
        }
        LerpRotate();
        LeftGun.transform.localEulerAngles = LeftTurrelUpdate();
        RightGun.transform.localEulerAngles = RightTurrelUpdate(); 
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(RailgunRotationPart.transform.rotation);
            stream.SendNext(LeftGun.transform.rotation);
            stream.SendNext(RightGun.transform.rotation);
        }
        else
        {
            var rotationPartRotation = (Quaternion) stream.ReceiveNext();
            var leftGunRotation = (Quaternion) stream.ReceiveNext();
            var rightGunRotation = (Quaternion)stream.ReceiveNext();
            
            rotationPartInterpolation.Update(() =>
            {
                RailgunRotationPart.transform.rotation = rotationPartRotation;
            }, info, rotationPartRotation);

            leftGunInterpolation.Update(() =>
            {
                LeftGun.transform.rotation = leftGunRotation; 
            }, info, leftGunRotation);

            rightGunInterpolation.Update(() =>
            {
                RightGun.transform.rotation = rightGunRotation;
            }, info, rightGunRotation);
        }
    }

    private Vector3 RotationUpdate()
    {
        if(GameManager.GameMode == GameMode.MultiPlayer)
            if (photonView.isMine == false) return Vector3.zero; 
        return new Vector3(
                RailgunRotationPart.transform.localEulerAngles.x,
                RailgunRotationPart.transform.localEulerAngles.y,
                Camera.transform.localEulerAngles.y
            );
    }

    private Vector3 LeftTurrelUpdate()
    {
        var cameraAngle = Camera.transform.localEulerAngles.x;
        return new Vector3(
           90 - cameraAngle - 45, 
           LeftGun.transform.localEulerAngles.y,
           LeftGun.transform.localEulerAngles.z);
    }

    private Vector3 RightTurrelUpdate()
    {
        var cameraAngle = Camera.transform.localEulerAngles.x;
        return new Vector3(
           cameraAngle - 45,
           RightGun.transform.localEulerAngles.y,
           RightGun.transform.localEulerAngles.z);
    }

    private void LerpRotate()
    {
        var targetAngle = nextRotation.z;
        var currentAngle = RailgunRotationPart.transform.localEulerAngles.z;

        if (targetAngle - currentAngle > 0 && Mathf.Abs(targetAngle - currentAngle) <= 180)
            //from current to target, forward 
            RailgunRotationPart.transform.localEulerAngles =
                Vector3.RotateTowards(RailgunRotationPart.transform.localEulerAngles, nextRotation,
                    Time.deltaTime * rotationSmooth, Time.deltaTime * 100f);
        else if (currentAngle - targetAngle >= 0 && Mathf.Abs(currentAngle - targetAngle) <= 180)
            //from target to current, back 
            RailgunRotationPart.transform.localEulerAngles =
                Vector3.RotateTowards(RailgunRotationPart.transform.localEulerAngles, nextRotation,
                    Time.deltaTime * rotationSmooth, Time.deltaTime * 100f);
        else
            RailgunRotationPart.transform.localEulerAngles =
                Vector3.RotateTowards(RailgunRotationPart.transform.localEulerAngles, nextRotation,
                    Time.deltaTime * rotationSmooth, -Time.deltaTime * 100f);
    }

}
