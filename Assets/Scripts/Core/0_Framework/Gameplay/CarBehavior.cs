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
        private CarEngine _engine = new CarEngine();


        //Input variables
        private Vector2 _inputDirection;
        private float _inputAcceleration;
        private float _inputBrake;
        private float _inputClutch = 1f;

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
                Debug.Log("Maquina de estado inicializada" + (_stateMachine != null));
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
            if (_inputDirection.magnitude > 0)
            {
                if (maxAngle * _inputDirection.x > steerAngle)
                {
                    steerAngle += steerSpeed * Time.deltaTime * 10;
                }
                else if (maxAngle * _inputDirection.x < steerAngle)
                {
                    steerAngle -= steerSpeed * Time.deltaTime * 10;
                }
            }
            else
            {
                steerAngle = Mathf.Lerp(steerAngle, 0, Time.deltaTime * steerSpeed);
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
            try
            {
                ServiceLocator.Instance.GetService<SetRpmUseCase>().SetRpmValue(_engineRPM);
            }
            catch { }
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

        public float GetWheelRPM()
        {
            float wheelRPM = (rearLeftWheelCollider.rpm + rearRightWheelCollider.rpm) / 2f;
            return wheelRPM;
        }

        public float GetCombustionTorque(float acceleration)
        {
            float combustionTorque = _torqueCurve.Evaluate(GetRPM()) * acceleration;
            return combustionTorque;
        }
        public float GetGearNetRatio()
        {
            float gearNetRatio = GetCurrentGearRatio() * _finalDriveRatio;
            return gearNetRatio;
        }
        public float GetCurrentTransmissionOmega()
        {
            float wheelOmega = GetWheelRPM() * 0.10472f;//(2 * Mathf.PI / 60f);
            return wheelOmega * GetCurrentGearRatio() * _finalDriveRatio;
        }
        public float GetLoadTorque()
        {
            if (GetCurrentGearRatio() == 0f)
                return 0f;

            float damping = 1.1f; // Coeficiente de amortiguamiento para simular la resistencia del motor

            float loadTorque = (_engine.FlyWheelAngularVelocity - GetCurrentTransmissionOmega()) * _inputClutch * damping;
            return loadTorque;
        }

        public float GetFrictionTorque()
        {
            float frictionTorque = _engine.FlyWheelAngularVelocity * 0.05f;
            return frictionTorque;
        }

        public float GetNetTorque(float acceleration)
        {
            float netTorque = GetCombustionTorque(acceleration) - GetFrictionTorque() - GetLoadTorque();
            return netTorque;
        }

        public float GetAccelerationTorque(float acceleration)
        {
            float accelerationTorque = GetNetTorque(acceleration) / _engine.inertia;
            return accelerationTorque;
        }

        public void SetEngineOmega(float accelerationInput, float extraTorque = 0f)
        {
            float combustionTorque = GetCombustionTorque(accelerationInput) + extraTorque;

            float netTorque = combustionTorque - GetFrictionTorque() - GetLoadTorque();

            float accelerationTorque = netTorque / _engine.inertia;
            _engine.FlyWheelAngularVelocity += accelerationTorque * Time.deltaTime;
            _engine.FlyWheelAngularVelocity = Mathf.Max(0f, _engine.FlyWheelAngularVelocity);


            float currentRPM = _engine.FlyWheelAngularVelocity * 9.5493f;// (60/(2 * Mathf.PI));
            SetRPM(currentRPM);
        }

        public void SetEngineOmegaFromIdle(float idleTorque)
        {
            SetEngineOmega(0f, idleTorque);
        }

        public void SetWheelTorque(float torque)
        {
            rearLeftWheelCollider.motorTorque = torque/2f;
            rearRightWheelCollider.motorTorque = torque/2f;
        }

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
                    float wheelRPM = (rearLeftWheelCollider.rpm + rearRightWheelCollider.rpm) / 2f;
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
                if (GetCurrentGearRatio() != 0)
                    _engineRPM = Mathf.Clamp(_engineRPM * 1.5f, _minRPM, _maxRPM);
                _currentGear--;

            }
            ServiceLocator.Instance.GetService<SetGearUseCase>().SetGearValue(_currentGear);
            Debug.Log("Shift Down");
        }

        internal void SetCarUISpeed()
        {

            float frontWheelRPM = (frontLeftWheelCollider.rpm + frontRightWheelCollider.rpm) / 2f;
            //float wheelCircumference = 2 * Mathf.PI * frontLeftWheelCollider.radius;
            float wheelCircumference = 2 * Mathf.PI * 0.41f;
            float speed = (frontWheelRPM * wheelCircumference * 60f) / 1000f;
            try
            {
                ServiceLocator.Instance.GetService<SetSpeedUseCase>().SetSpeedValue(speed);
            }
            catch { }
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
            SetCarUISpeed();

        }
        internal void FixedTick()
        {
            _stateMachine.Tick();
        }
    }
}