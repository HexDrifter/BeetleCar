using UnityEngine;
using DG.Tweening;
using System;


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
        
        [Header("Exterior")]
        [SerializeField] private Transform _frontalHood;
        [SerializeField] private Transform _rearHood;
        
        [Header("Interior")]
        [SerializeField] private Transform _steeringWheel;
        private bool _rearHoodOpen;

        [Header("Engine Parts")]
        [SerializeField] private float _strokeDistance = .15f;
        [SerializeField] private Transform _flyWheel;
        [SerializeField] private Transform _piston1;
        [SerializeField] private Transform _piston2;
        [SerializeField] private Transform _piston3;
        [SerializeField] private Transform _piston4;

        private void Start()
        {
            if (_carBehavior == null)
            {
                _carBehavior = GetComponent<CarBehavior>();
            }
        }
        private void Update()
        {
            AnimateWheels();
            AnimateEngine();
        }

        private void LateUpdate()
        {
            AnimateSteeringWheel();
        }

        private void AnimateSteeringWheel()
        {
            _steeringWheel.localRotation = Quaternion.Euler(14, -90, -_carBehavior.steerAngle*10f);
        }
        private void AnimateEngine()
        {
            var angularVelocity = _carBehavior.Engine.FlyWheelAngularVelocity;
            float currentPhase = _carBehavior.Engine.EnginePhase;


            _flyWheel.Rotate(Vector3.forward, angularVelocity * Mathf.Rad2Deg * Time.deltaTime);

            float oscilation1 = Mathf.Sin(currentPhase) *_strokeDistance;
            float oscilation2 = Mathf.Sin(currentPhase + Mathf.PI) *_strokeDistance;
            
            float[] offsets = {-0.1f,.1f};

            _piston1.localPosition = new Vector3(oscilation1 + offsets[0], _piston1.localPosition.y, _piston1.localPosition.z);
            _piston2.localPosition = new Vector3(-oscilation1 + offsets[1], _piston2.localPosition.y, _piston2.localPosition.z);
            _piston3.localPosition = new Vector3(oscilation2 + offsets[0], _piston3.localPosition.y, _piston3.localPosition.z);
            _piston4.localPosition = new Vector3(-oscilation2 + offsets[1], _piston4.localPosition.y, _piston4.localPosition.z);

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

            //frontLeftWheelRotation *= Quaternion.Euler(0, 0, 00);
            //frontRightWheelRotation *= Quaternion.Euler(0, 0, 90);

            _frontLeftWheel.transform.rotation = frontLeftWheelRotation;
            _frontRightWheel.transform.rotation = frontRightWheelRotation;

            _frontLeftWheel.transform.position = frontLeftWheelPose;
            _frontRightWheel.transform.position = frontRightWheelPose;

            _carBehavior.rearLeftWheelCollider.GetWorldPose(out Vector3 rearLeftWheelPose, out Quaternion rearLeftWheelRotation);
            _carBehavior.rearRightWheelCollider.GetWorldPose(out Vector3 rearRightWheelPose, out Quaternion rearRightWheelRotation);

            //rearLeftWheelRotation *= Quaternion.Euler(0, 0, 90);
            //rearRightWheelRotation *= Quaternion.Euler(0, 0, 90);
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
