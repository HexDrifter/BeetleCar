using UnityEngine;
using DG.Tweening;


namespace Beetle.Framework
{
    public class CarAnimator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private CarBehavior _carBehavior;
        [SerializeField] private bool      _hasCalliper;
        [Header("Wheel Transform")]
        [SerializeField] private Transform _frontLeftWheel;
        [SerializeField] private Transform _frontRightWheel;
        [SerializeField] private Transform _rearLeftWheel;
        [SerializeField] private Transform _rearRightWheel;
        [Header("Calliper Transform")]
        [SerializeField] private Transform _frontLeftCalliper;
        [SerializeField] private Transform _frontRightCalliper;
        [SerializeField] private Transform _rearLeftCalliper;
        [SerializeField] private Transform _rearRightCalliper;
        [Header("Hood")]
        [SerializeField] private Transform _frontalHood;
        [SerializeField] private Transform _rearHood;

        private bool _rearHoodOpen;

        private void Update()
        {
            AnimateWheels();
        }
        public void OpenCloseRearHood(bool opening)
        {
            if (opening)
            {
                if (!_rearHoodOpen)
                {
                    // Rotación local relativa al objeto _rearHood
                    Quaternion quaternion = _rearHood.localRotation * Quaternion.Euler(0f, 0f, 100f);
                    _rearHood.DOLocalRotateQuaternion(quaternion, 1.5f);

                    //_rearHood.Rotate(Vector3.right, 100f);
                    _rearHoodOpen = true;
                }
                else
                {
                    // Volver a la rotación original (local)
                    Quaternion quaternion = _rearHood.localRotation * Quaternion.Euler(0f, 0f, -100f);
                    _rearHood.DOLocalRotateQuaternion(quaternion, 1.5f);
                    //_rearHood.Rotate(Vector3.right, -100f);
                    _rearHoodOpen = false;
                }
            }
        }
        internal void AnimateWheels()
        {
            _carBehavior.frontLeftWheelCollider.GetWorldPose(out Vector3 frontLeftWheelPose, out Quaternion frontLeftWheelRotation);
            _carBehavior.frontRightWheelCollider.GetWorldPose(out Vector3 frontRightWheelPose, out Quaternion frontRightWheelRotation);

            frontLeftWheelRotation *= Quaternion.Euler(0, 0, 90);
            frontRightWheelRotation *= Quaternion.Euler(0, 0, 90);

            _frontLeftWheel.transform.rotation = frontLeftWheelRotation;
            _frontRightWheel.transform.rotation = frontRightWheelRotation;

            _frontLeftWheel.transform.position = frontLeftWheelPose;
            _frontRightWheel.transform.position = frontRightWheelPose;

            _carBehavior.rearLeftWheelCollider.GetWorldPose(out Vector3 rearLeftWheelPose, out Quaternion rearLeftWheelRotation);
            _carBehavior.rearRightWheelCollider.GetWorldPose(out Vector3 rearRightWheelPose, out Quaternion rearRightWheelRotation);

            rearLeftWheelRotation *= Quaternion.Euler(0, 0, 90);
            rearRightWheelRotation *= Quaternion.Euler(0, 0, 90);
            _rearLeftWheel.transform.rotation = rearLeftWheelRotation;
            _rearRightWheel.transform.rotation = rearRightWheelRotation;

            _rearLeftWheel.transform.position = rearLeftWheelPose;
            _rearRightWheel.transform.position = rearRightWheelPose;

            if (_hasCalliper)
            {
                _frontLeftCalliper.transform.position = frontLeftWheelPose;
                _frontLeftCalliper.transform.rotation = Quaternion.Euler(0, frontLeftWheelRotation.eulerAngles.y, 0);

                _frontRightCalliper.transform.position = frontRightWheelPose;
                _frontRightCalliper.transform.rotation = Quaternion.Euler(0, frontRightWheelRotation.eulerAngles.y, 0);
                _rearLeftCalliper.transform.position = rearLeftWheelPose;
                _rearLeftCalliper.transform.rotation = Quaternion.Euler(0, rearLeftWheelRotation.eulerAngles.y, 0);

                _rearRightCalliper.transform.position = rearRightWheelPose;
                _rearRightCalliper.transform.rotation = Quaternion.Euler(0, rearRightWheelRotation.eulerAngles.y, 0);
            }
        }
    }
}
