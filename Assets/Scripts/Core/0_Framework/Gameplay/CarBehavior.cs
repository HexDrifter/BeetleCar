using ClownCar.Domain;
using ClownCar.SystemUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace ClownCar.Framework
{

    public class CarBehavior : MonoBehaviour
    {
        private int _currentGear = 1;
        private int _maxGear = 5 ;
        private float[] _gearRatios = { -3.6f, 0f, 3.8f, 2.06f, 1.32f, 0.89f };
        private float _finalDriveRatio = 4.12f;
        private float _engineRPM; 
        private float _minRPM = 700f;
        private float _maxRPM = 5000f;
        private float steerAngle;

        


        private Vector2 _inputDirection;
        private float _inputAcceleration;
        private float _inputBrake;


        [Header("Config")]
        [SerializeField] public float engineTorque;
        [SerializeField] public float brakeForce;
        [SerializeField] public float maxAngle;
        [SerializeField] public float steerSpeed;
    
        public Vector2 inputDirection => _inputDirection;
        public float inputAcceleration => _inputAcceleration;
        public float inputBrake => _inputBrake;
        public float engineRPM => _engineRPM;
        public int currentGear => _currentGear;


        [Header("Wheel Colliders")]
        [SerializeField] public WheelCollider frontLeftWheelCollider;
        [SerializeField] public WheelCollider frontRightWheelCollider;
        [SerializeField] public WheelCollider rearLeftWheelCollider;
        [SerializeField] public WheelCollider rearRightWheelCollider;

        [Header("Wheel Transform")]
        [SerializeField] public Transform frontLeftWheel;
        [SerializeField] public Transform frontRightWheel;
        [SerializeField] public Transform rearLeftWheel;
        [SerializeField] public Transform rearRightWheel;

        [Header("Wheel Calipers")]
        [SerializeField] public Transform frontLeftCaliper;
        [SerializeField] public Transform frontRightCaliper;
        [SerializeField] public Transform rearLeftCaliper;
        [SerializeField] public Transform rearRightCaliper;

        internal void SetInputDirection(Vector2 inputDirection)
        {
            _inputDirection = inputDirection;
        }

        internal void SetCarDirection()
        {
            
            if (maxAngle * _inputDirection.x > steerAngle)
            {
                steerAngle += steerSpeed;
            }
            else if(maxAngle * _inputDirection.x < steerAngle)
            {
                steerAngle -= steerSpeed;
            }
            frontLeftWheelCollider.steerAngle = steerAngle;
            frontRightWheelCollider.steerAngle = steerAngle;
        }

        internal void SetInputAcceleration(float inputAcceleration)
        {

            _inputAcceleration = inputAcceleration;
        }

        internal void CalculateEngineRPM()
        {
            float wheelRPM = (rearLeftWheelCollider.rpm + rearRightWheelCollider.rpm) / 2f;
            _engineRPM = wheelRPM * _gearRatios[_currentGear]* _finalDriveRatio;
            ServiceLocator.Instance.GetService<SetRpmUseCase>().SetRpmValue(_engineRPM);
        }


        internal void SetCarAcceleration()
        {
            if (_gearRatios[_currentGear] != 0f)
            {
                if (_engineRPM < _maxRPM)
                {
                    float torque = engineTorque * _inputAcceleration * _gearRatios[_currentGear];
                    rearLeftWheelCollider.motorTorque = torque;
                    rearRightWheelCollider.motorTorque = torque;

                }
                else
                {
                    float torque = engineTorque * 0 * _gearRatios[_currentGear];
                    rearLeftWheelCollider.motorTorque = torque;
                    rearRightWheelCollider.motorTorque = torque;
                }

            }
            else
            {
                rearLeftWheelCollider.motorTorque = 0f;
                rearRightWheelCollider.motorTorque = 0f;
            }
        }

        internal void SetInputBrake(float inputBrake)
        {
            _inputBrake = inputBrake;
        }

        internal void SetCarBrake()
        {
            frontLeftWheelCollider.brakeTorque = brakeForce * _inputBrake;
            frontRightWheelCollider.brakeTorque = brakeForce * _inputBrake;
            rearLeftWheelCollider.brakeTorque = brakeForce * _inputBrake;
            rearRightWheelCollider.brakeTorque = brakeForce * _inputBrake;
        
        }

        internal void AnimateWheels()
        {
            frontLeftWheelCollider.GetWorldPose(out Vector3 frontLeftWheelPose, out Quaternion frontLeftWheelRotation);
            frontRightWheelCollider.GetWorldPose(out Vector3 frontRightWheelPose, out Quaternion frontRightWheelRotation);

            frontLeftWheelRotation *= Quaternion.Euler(0, -90, 0);
            frontRightWheelRotation *= Quaternion.Euler(0, 90, 0);
        
            frontLeftWheel.transform.rotation = frontLeftWheelRotation;
            frontRightWheel.transform.rotation = frontRightWheelRotation;
        
            frontLeftWheel.transform.position = frontLeftWheelPose;
            frontRightWheel.transform.position = frontRightWheelPose;
        
            frontLeftCaliper.transform.position = frontLeftWheelPose;
            frontLeftCaliper.transform.rotation = Quaternion.Euler(0, frontLeftWheelRotation.eulerAngles.y, 0);

            frontRightCaliper.transform.position = frontRightWheelPose;
            frontRightCaliper.transform.rotation = Quaternion.Euler(0, frontRightWheelRotation.eulerAngles.y, 0);



            rearLeftWheelCollider.GetWorldPose(out Vector3 rearLeftWheelPose, out Quaternion rearLeftWheelRotation);
            rearRightWheelCollider.GetWorldPose(out Vector3 rearRightWheelPose, out Quaternion rearRightWheelRotation);
        
            rearLeftWheelRotation *= Quaternion.Euler(0, -90, 0);
            rearRightWheelRotation *= Quaternion.Euler(0, 90, 0);
        
            rearLeftWheel.transform.rotation = rearLeftWheelRotation;
            rearRightWheel.transform.rotation = rearRightWheelRotation;

            rearLeftWheel.transform.position = rearLeftWheelPose;
            rearRightWheel.transform.position = rearRightWheelPose;

            rearLeftCaliper.transform.position = rearLeftWheelPose;
            rearLeftCaliper.transform.rotation = Quaternion.Euler(0, rearLeftWheelRotation.eulerAngles.y, 0);

            rearRightCaliper.transform.position = rearRightWheelPose;
            rearRightCaliper.transform.rotation = Quaternion.Euler(0, rearRightWheelRotation.eulerAngles.y, 0);
        }

        internal void ShiftUp()
        {
            if (_currentGear < _maxGear)
            {
                _currentGear++;
                _engineRPM = Mathf.Clamp(_engineRPM * 0.7f, _minRPM, _maxRPM);
                ServiceLocator.Instance.GetService<SetGearUseCase>().SetGearValue(_currentGear);
            }

        }

        internal void ShiftDown()
        {
            if (_currentGear > 0)
            {
                _currentGear--;
                _engineRPM = Mathf.Clamp(_engineRPM * 1.5f, _minRPM, _maxRPM);
            }
            ServiceLocator.Instance.GetService<SetGearUseCase>().SetGearValue(_currentGear);
        }
        internal void tick()
        {
            SetCarDirection();
            SetCarAcceleration();
            SetCarBrake();
            AnimateWheels();
            CalculateEngineRPM();
        }

    
    }

}