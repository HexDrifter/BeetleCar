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
        private float _minRPM = 800f;
        private float _maxRPM = 6000f;
        private float steerAngle;
        private bool _inputEngineIgnition;
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
        [SerializeField] private AnimationCurve _torqueCurve;
    
        public Vector2 inputDirection => _inputDirection;
        public float inputAcceleration => _inputAcceleration;
        public float inputBrake => _inputBrake;
        public float engineRPM => _engineRPM;
        public int currentGear => _currentGear;
        public float carSpeed => _carSpeed;
        public float maxRPM => _maxRPM;
        public float minRPM => _minRPM;
        public float[] gearRatios => _gearRatios;
        public float finalDriveRatio => _finalDriveRatio;
        public AnimationCurve torqueCurve => _torqueCurve;
        public bool inputEngineIgnition => _inputEngineIgnition;



        [Header("Wheel Colliders")]
        [SerializeField] public WheelCollider frontLeftWheelCollider;
        [SerializeField] public WheelCollider frontRightWheelCollider;
        [SerializeField] public WheelCollider rearLeftWheelCollider;
        [SerializeField] public WheelCollider rearRightWheelCollider;

        protected string AtEngineState(IState from, IState to, Func<bool> condition) => _stateMachine.AddTransition(from, to, condition); 

        public virtual void Initialize()
        {
            _stateMachine = new StateMachine();
            if (_stateMachine != null)
            {
                Debug.Log("Maquina de estado inicializada"+ (_stateMachine != null));

            }
            else
            {
                Debug.Log("Maquina de estado NO inicializada");
            }
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

        internal void SetInputEngineIgnition(bool inputEngineIgnition)
        {
            _inputEngineIgnition = inputEngineIgnition;
        }

        public void SetRPM(float rpm)
        {
            _engineRPM = rpm;
            ServiceLocator.Instance.GetService<SetRpmUseCase>().SetRpmValue(_engineRPM);
        }
        public float GetRPM()
        {
            return _engineRPM;
        }
        public float GetCurrentGearRatio()
        {
            float gearRatio = _gearRatios[_currentGear];
            return gearRatio;
        }
        internal void SetEngineRPM()
        {
            if (_gearRatios[_currentGear] == 0f)//Neutro
            {
                if (_inputAcceleration > .1f)//Acelerando
                {
                    _engineRPM = Mathf.Clamp(
                        _engineRPM + (engineTorque * _inputAcceleration * Time.deltaTime * 10),
                        _minRPM, _maxRPM
                        );
                }
                else //Decelerando
                {
                    _engineRPM = Mathf.Lerp(
                        _engineRPM,
                        _minRPM + UnityEngine.Random.Range(-50f, 50f),
                        Time.deltaTime *2f
                        );
                }
            }
            else //Con cambio
            {
                float wheelRPM = (rearLeftWheelCollider.rpm + rearRightWheelCollider.rpm) / 2f;
                float calculatedRPM = wheelRPM * _gearRatios[_currentGear] * _finalDriveRatio;

                
                if (_inputAcceleration > 0.1f)// Acelerando
                {
                    _engineRPM = Mathf.Lerp(
                        _engineRPM,
                        Mathf.Clamp(calculatedRPM + (engineTorque * _inputAcceleration), _minRPM, _maxRPM),
                        Time.deltaTime * 5f
                    );
                }
                else //Desacelerando
                {
                    _engineRPM = Mathf.Lerp(_engineRPM, calculatedRPM, Time.deltaTime * 3f);
                }
            }
            ServiceLocator.Instance.GetService<SetRpmUseCase>().SetRpmValue(_engineRPM);
        }

        public float GetWheelRPM()
        {
            float wheelRPM = (rearLeftWheelCollider.rpm + rearRightWheelCollider.rpm) / 2f;
            return wheelRPM; 
        }

        public void SetWheelTorque(float torque)
        {
            rearLeftWheelCollider.motorTorque = torque;
            rearRightWheelCollider.motorTorque = torque;
        }
        /*public void SetCarAcceleration()
        {
            float torque;
            if (_gearRatios[_currentGear] != 0f)
            {
                torque = _torqueCurve.Evaluate(_engineRPM/ _maxRPM) * _inputAcceleration * _gearRatios[_currentGear];
            }
            else
            {
                torque = 0;
            }
            rearLeftWheelCollider.motorTorque = torque;
            rearRightWheelCollider.motorTorque = torque;
        }*/

        internal void SetInputBrake(float inputBrake)
        {
            _inputBrake = inputBrake;
        }

        internal void SetCarBrake()
        {
            float brakeTorque = brakeForce * _inputBrake;
            frontLeftWheelCollider.brakeTorque = brakeTorque;
            frontRightWheelCollider.brakeTorque = brakeTorque;
            rearLeftWheelCollider.brakeTorque = brakeTorque;
            rearRightWheelCollider.brakeTorque = brakeTorque;
        
        }


        internal void ShiftUp()
        {
            if (_currentGear < _maxGear)
            {
                _currentGear++;
                if (_gearRatios[_currentGear] != 0f)
                {
                    float wheelRPM = (rearLeftWheelCollider.rpm + rearRightWheelCollider.rpm ) / 2f;
                    _engineRPM = Mathf.Clamp(wheelRPM * _gearRatios[_currentGear] * _finalDriveRatio, _minRPM, _maxRPM);
                }
                ServiceLocator.Instance.GetService<SetGearUseCase>().SetGearValue(_currentGear);
            }
            Debug.Log("Shift Up");
        }

        internal void ShiftDown()
        {
            if (_currentGear > 0)
            {
                if(GetCurrentGearRatio() != 0)
                    _engineRPM = Mathf.Clamp(_engineRPM * 1.5f, _minRPM, _maxRPM);
                _currentGear--;

            }
            ServiceLocator.Instance.GetService<SetGearUseCase>().SetGearValue(_currentGear);
            Debug.Log("Shift Down");
        }

        internal void SetCarSpeed()
        {

            float frontWheelRPM = (frontLeftWheelCollider.rpm + frontRightWheelCollider.rpm) / 2f;
            //float wheelCircumference = 2 * Mathf.PI * frontLeftWheelCollider.radius;
            float wheelCircumference = 2 * Mathf.PI * 0.41f;
            float speed = (frontWheelRPM * wheelCircumference * 60f) / 1000f;
            ServiceLocator.Instance.GetService<SetSpeedUseCase>().SetSpeedValue(speed);
        }

        public float GetCarSpeed()
        {
            float frontWheelRPM = (frontLeftWheelCollider.rpm + frontRightWheelCollider.rpm) / 2f;
            float wheelCircumference = 2 * Mathf.PI * 0.41f;
            float speed = (frontWheelRPM * wheelCircumference * 60f) / 1000f;
            return speed;
        }
        internal void setGlobalSpeed()
        {

        }
        internal void Tick()
        {
            SetCarDirection();
            //SetCarAcceleration();
            SetCarBrake();
            //SetEngineRPM();
            SetCarSpeed();
            _stateMachine.Tick();

        }

    
    }

}