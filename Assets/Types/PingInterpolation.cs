using System;
using UnityEngine;

namespace Assets.W.Types
{
    public class PingInterpolation<T>
    {
        public T HostObject { get; private set; }

        private bool _isFirstPackage = true;
        private double _lastPackageTimestamp = -1;
        private double _currentPackageTimestamp = -1;
        private double[] _timeParams;

        public void Update(Action firstPackage, PhotonMessageInfo info, T value)
        {
            if (_isFirstPackage)
            {
                _isFirstPackage = false;
                _lastPackageTimestamp = info.timestamp;
                firstPackage.Invoke();
            }
            else
            {
                var previousTimeStamp = _currentPackageTimestamp;
                var currentTimeStamp = info.timestamp;
                if (Math.Abs(previousTimeStamp - (-1)) < 0.01)
                    _currentPackageTimestamp = currentTimeStamp = _lastPackageTimestamp;
                _currentPackageTimestamp = info.timestamp;
                _timeParams = new[] { previousTimeStamp, currentTimeStamp };
                HostObject = value;
            }
        }

        public void Interpolate(Action<T, float> interpolation, float timeDelay)
        {
            if (_timeParams == null) return;
            var ping = (float) (_timeParams[1] - _timeParams[0]);
            var pingSmooth = timeDelay/ping; 
            interpolation.Invoke(HostObject, pingSmooth);
        }
    }
}
