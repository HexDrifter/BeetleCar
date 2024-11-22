using Beetle.Domain;
using Beetle.Entities;
using Beetle.SystemUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace Beetle.Framework
{

    public class CarBehavior : MonoBehaviour
    {
        protected StateMachine _stateMachine;


        private int _currentGear = 1;
        private int _maxGear = 5;
        private float[] _gearRatios = { -3.6f, 0f, 3.8f, 2.06f, 1.32f, 0.89f };
        private float _finalDriveRatio = 4.12f;
        private float _engineRPM;
        private float _carSpeed;
        private float _minRPM = 700f;
        private float _maxRPM = 6600f;
        private float steerAngle;
        private Car _car;
        private CarAnimator _carAnimator;
        


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
        public float carSpeed => _carSpeed;
        public float maxRPM => _maxRPM;


        [Header("Wheel Colliders")]
        [SerializeField] public WheelCollider frontLeftWheelCollider;
        [SerializeField] public WheelCollider frontRightWheelCollider;
        [SerializeField] public WheelCollider rearLeftWheelCollider;
        [SerializeField] public WheelCollider rearRightWheelCollider;

        protected string AtEngineState(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition); 

        public virtual void Initialize()
        {
            _stateMachine = new StateMachine();
        }

        internal void SetInputDirection(Vector2 inputDirection)
        {
            _inputDirection = inputDirection;
        }

        internal void SetCarDirection()
        {
            
            if (maxAngle * _inputDirection.x > steerAngle)
            {
                steerAngle += steerSpeed * Time.deltaTime * 10;
            }
            else if(maxAngle * _inputDirection.x < steerAngle)
            {
                steerAngle -= steerSpeed * Time.deltaTime * 10;
            }
            frontLeftWheelCollider.steerAngle = steerAngle;
            frontRightWheelCollider.steerAngle = steerAngle;
        }

        internal void SetInputAcceleration(float inputAcceleration)
        {

            _inputAcceleration = inputAcceleration;
        }

        internal void SetEngineRPM()
        {
            if(_gearRatios[_currentGear] != 0f)
            {
                float wheelRPM = (rearLeftWheelCollider.rpm + rearRightWheelCollider.rpm) / 2f;
                _engineRPM = wheelRPM * _gearRatios[_currentGear]* _finalDriveRatio;

            }
            else if (_engineRPM >= _minRPM)
            {
                if(_engineRPM < _maxRPM)
                {
                    _engineRPM +=(engineTorque * _inputAcceleration) - 50f;
                }
                else
                {
                    _engineRPM -= 50f;
                }
            }
            else
            {
                _engineRPM = _minRPM;
            }
            ServiceLocator.Instance.GetService<SetRpmUseCase>().SetRpmValue(_engineRPM);
        }




        internal void SetCarAcceleration()
        {

            if (_gearRatios[_currentGear] != 0f)
            {
                float torque;
                if (_engineRPM < _minRPM && _inputAcceleration <= 0.1f)
                {
                    torque = (engineTorque * 0.5f * _gearRatios[_currentGear]);
                }
                else if(_engineRPM < _maxRPM){
                    if (_inputAcceleration >= 0.1f)
                    {
                        torque = (engineTorque * _inputAcceleration * _gearRatios[_currentGear]);
                    }
                    else
                    {
                        torque =  - 70f;
                    }
                }
                else
                {
                    torque =-70f;
                }
                rearLeftWheelCollider.motorTorque = torque;
                rearRightWheelCollider.motorTorque = torque;
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

        internal void SetCarSpeed()
        {

            float frontWheelRPM = (frontLeftWheelCollider.rpm + frontRightWheelCollider.rpm) / 2f;
            //float wheelCircumference = 2 * Mathf.PI * frontLeftWheelCollider.radius;
            float wheelCircumference = 2 * Mathf.PI * 0.41f;
            float speed = (frontWheelRPM * wheelCircumference * 60f) / 1000f;
            ServiceLocator.Instance.GetService<SetSpeedUseCase>().SetSpeedValue(speed);
        }
        internal void Tick()
        {
            SetCarDirection();
            SetCarAcceleration();
            SetCarBrake();
            SetEngineRPM();
            SetCarSpeed();
        }

    
    }

}