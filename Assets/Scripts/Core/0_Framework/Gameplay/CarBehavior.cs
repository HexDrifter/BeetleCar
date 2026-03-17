using Beetle.Domain;
using Beetle.Entities;
using Beetle.SystemUtilities;
using System;
using DG.Tweening;
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
        [Header("Debug")]
        public float Damping;
        //Input variables
        private Vector2 _inputDirection;
        private float _inputAcceleration;
        private float _inputBrake;
        private float _inputClutch = 1f;
        private float _carSpeed;
        private float _steerAngle;
        private bool _inputEngineIgnition;
        private CarEngine _engine = new CarEngine(new float[] { -3.6f, 0f, 3.8f, 2.06f, 1.32f, 0.89f }, 4.12f);

        [Header("Config")]
        [SerializeField] public float brakeForce;
        [SerializeField] public float maxAngle;
        [SerializeField] public float steerSpeed;
        [SerializeField] private bool autoShift = true;
        [SerializeField] private AnimationCurve _torqueCurve;

        public Vector2 inputDirection => _inputDirection;
        public float steerAngle => _steerAngle;
        public float inputAcceleration => _inputAcceleration;
        public float inputBrake => _inputBrake;
        public float inputClutch => _inputClutch;
        public float carSpeed => _carSpeed;
        public AnimationCurve torqueCurve => _torqueCurve;
        public bool inputEngineIgnition => _inputEngineIgnition;
        public CarEngine Engine => _engine;

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
                    _steerAngle += steerSpeed * Time.deltaTime * 10;
                }
                else if (maxAngle * _inputDirection.x < steerAngle)
                {
                    _steerAngle -= steerSpeed * Time.deltaTime * 10;
                }
            }
            else
            {
                _steerAngle = Mathf.Lerp(steerAngle, 0, Time.deltaTime * steerSpeed);
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
            try
            {
                ServiceLocator.Instance.GetService<SetRpmUseCase>().SetRpmValue(rpm);
            }
            catch { }
        }

        public float GetRPM()
        {
            return _engine.FlyWheelAngularVelocity * 9.5493f;
        }

        public float GetCurrentGearRatio()
        {
            float gearRatio = _engine.GearRatios[_engine.CurrentGear];
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
            return _engine.CurrentGearRatio * _engine.FinalDriveRatio;
        }

        public float GetCurrentTransmissionOmega()
        {
            return GetWheelRPM() * 0.10472f * GetGearNetRatio();//(2 * Mathf.PI / 60f);
        }

        public float GetLoadTorque()
        {
            float gearNetRatio = GetGearNetRatio();
            if (gearNetRatio == 0)
                return 0f;

            //float damping = 1.0f; // Coeficiente de amortiguamiento para simular la resistencia del motor
            float transmissionOmega = GetCurrentTransmissionOmega();
            float difference = _engine.FlyWheelAngularVelocity - transmissionOmega;
            float loadTorque = difference * _inputClutch * Damping;
            return Mathf.Clamp(loadTorque, -2000f, 2000f);
        }

        public float GetFrictionTorque()
        {
            float frictionTorque = _engine.FlyWheelAngularVelocity * 0.1f;
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
            _engine.EnginePhase += _engine.FlyWheelAngularVelocity * Time.deltaTime;

            float explosionStroke = Mathf.Pow(Mathf.Abs(Mathf.Sin(_engine.EnginePhase)), 8f);

            float combustionTorque = GetCombustionTorque(accelerationInput) * explosionStroke * 3.5f;
            combustionTorque += extraTorque;

            float netTorque = combustionTorque - GetFrictionTorque() - GetLoadTorque();
            Debug.Log($"Combustion Torque: {combustionTorque}, Friction Torque: {GetFrictionTorque()}, Net Torque: {netTorque}");
            float accelerationTorque = netTorque / _engine.inertia;
            _engine.FlyWheelAngularVelocity += accelerationTorque * Time.deltaTime;

            if (_engine.FlyWheelAngularVelocity < 0.1f || float.IsNaN(_engine.FlyWheelAngularVelocity))
            {
                _engine.FlyWheelAngularVelocity = 0f;
            }

            _engine.FlyWheelAngularVelocity = Mathf.Clamp(_engine.FlyWheelAngularVelocity, 0f, 1000f);

            float currentRPM = _engine.FlyWheelAngularVelocity * 9.5493f;// (60/(2 * Mathf.PI));
            SetRPM(currentRPM);
        }

        public void SetEngineOmegaFromIdle(float idleTorque)
        {
            SetEngineOmega(0f, idleTorque);
        }

        public void SetWheelTorque(float torque)
        {
            float smoothedTorque = Mathf.Lerp(rearLeftWheelCollider.motorTorque, torque / 2f, Time.deltaTime * 10f);
            rearLeftWheelCollider.motorTorque = smoothedTorque;
            rearRightWheelCollider.motorTorque = smoothedTorque;
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
            if (_engine.CurrentGear < _engine.MaxGears)
            {
                Debug.Log("Cambiando a marcha: " + (_engine.CurrentGear + 1));
                ExcecuteShift(_engine.CurrentGear + 1);
            }
        }

        internal void ShiftDown()
        {
            if (_engine.CurrentGear > 0)
            {
                Debug.Log("Reduciendo a marcha: " + (_engine.CurrentGear - 1));
                ExcecuteShift(_engine.CurrentGear - 1);
            }
        }

        private void ExcecuteShift(int targetGear)
        {
            if (autoShift)
            {
                _inputClutch = 0f;
                _engine.CurrentGear = targetGear;
                ServiceLocator.Instance.GetService<SetGearUseCase>().SetGearValue(_engine.CurrentGear);

                DOTween.To(() => _inputClutch, x => _inputClutch = x, 1f, 0.5f).SetEase(Ease.OutQuad)
                    .OnComplete(() =>
                    {
                        Debug.Log("Embrague Liberado");
                    });
            }
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
            SetCarBrake();
            SetCarUISpeed();

        }
        internal void FixedTick()
        {
            _stateMachine.Tick();
        }
    }
}