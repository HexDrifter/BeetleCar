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
        [Header("Powertrain")]
        [Tooltip("Coeficiente de acoplamiento viscoso del embrague (N·m por rad/s).")]
        [Min(0f)] public float Damping;
        [Tooltip("Torque máximo que puede transmitir el embrague completamente acoplado, en N·m.")]
        [SerializeField, Min(0f)] private float clutchTorqueCapacity;
        [Tooltip("Coeficiente de resistencia de compresión con el motor apagado.")]
        [SerializeField, Min(0f)] private float engineOffDragCoefficient = 3f;

        private Vector2 _inputDirection;
        private float _inputAcceleration;
        private float _inputBrake;
        private float _inputClutch = 1f;
        private float _carSpeed;
        private float _steerAngle;
        private bool _inputEngineIgnition;
        private int _powertrainDebugCounter;
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

        private const float RpmToRadiansPerSecond = 0.10471976f;

        public float GetRPM()
        {
            return _engine.CurrentRPM;
        }

        public float GetCurrentGearRatio()
        {
            return _engine.CurrentGearRatio;
        }

        public float GetWheelRPM()
        {
            return (rearLeftWheelCollider.rpm + rearRightWheelCollider.rpm) * 0.5f;
        }

        /// <summary>
        /// Torque medio del motor a las RPM actuales, escalado por la apertura del acelerador.
        /// La curva ya representa el torque medio: no debe volver a multiplicarse por una onda
        /// artificial de combustión.
        /// </summary>
        public float GetCombustionTorque(float acceleration)
        {
            if (_torqueCurve == null || _torqueCurve.length == 0)
                return 0f;

            float throttle = Mathf.Clamp01(acceleration);
            return _torqueCurve.Evaluate(GetRPM()) * throttle;
        }

        public float GetGearNetRatio()
        {
            return _engine.CurrentGearRatio * _engine.FinalDriveRatio;
        }

        /// <summary>
        /// Velocidad angular equivalente en el lado de entrada de la transmisión.
        /// Las RPM de las ruedas se convierten a rad/s antes de aplicar la relación total.
        /// </summary>
        public float GetCurrentTransmissionOmega()
        {
            return GetWheelRPM() * RpmToRadiansPerSecond * GetGearNetRatio();
        }

        /// <summary>
        /// Torque transmitido por el embrague hacia la transmisión, referido al eje del motor.
        /// El signo se obtiene de la diferencia de velocidades: puede impulsar o frenar el motor.
        /// </summary>
        public float GetLoadTorque()
        {
            float gearRatio = GetGearNetRatio();
            float clutchEngagement = Mathf.Clamp01(_inputClutch);

            if (Mathf.Approximately(gearRatio, 0f) || clutchEngagement <= 0f)
                return 0f;

            float engineOmega = _engine.FlyWheelAngularVelocity;
            float transmissionOmega = GetCurrentTransmissionOmega();
            float relativeOmega = engineOmega - transmissionOmega;

            float coupling = Mathf.Max(0f, Damping);
            float requestedTorque = relativeOmega * coupling;
            float availableTorque = Mathf.Max(0f, clutchTorqueCapacity) * clutchEngagement;

            // La capacidad finita representa el límite mecánico del embrague, no un límite
            // numérico arbitrario aplicado al torque de todo el motor.
            return Mathf.Clamp(requestedTorque, -availableTorque, availableTorque);
        }

        public float GetFrictionTorque()
        {
            // Modelo inicial de fricción viscosa interna. El torque se opone al giro positivo.
            return _engine.FlyWheelAngularVelocity * 0.1f;
        }

        public float GetNetTorque(float acceleration, float additionalTorque = 0f)
        {
            return GetCombustionTorque(acceleration)
                   + additionalTorque
                   - GetFrictionTorque()
                   - GetLoadTorque();
        }

        public float GetAccelerationTorque(float acceleration, float additionalTorque = 0f)
        {
            if (_engine.inertia <= 0f)
                return 0f;

            return GetNetTorque(acceleration, additionalTorque) / _engine.inertia;
        }

        public float GetWheelAngularVelocity(WheelCollider wheel)
        {
            return wheel.rpm * RpmToRadiansPerSecond;
        }

        public float GetDifferentialOmega()
        {
            float leftWheelOmega = GetWheelAngularVelocity(rearLeftWheelCollider);
            float rightWheelOmega = GetWheelAngularVelocity(rearRightWheelCollider);
            return (leftWheelOmega + rightWheelOmega) * 0.5f;
        }

        public float GetEngineTargetVelocity()
        {
            return GetDifferentialOmega() * GetGearNetRatio();
        }

        /// <summary>
        /// Avanza el motor y aplica a las ruedas el mismo torque de embrague usado en el balance
        /// del cigüeñal. Debe llamarse una vez por paso de física.
        /// </summary>
        public void SimulatePowertrain(
    float accelerationInput,
    float additionalTorque = 0f,
    bool combustionEnabled = true)
        {
            float deltaTime = Time.fixedDeltaTime;

            if (deltaTime <= 0f)
                return;

            // Validar referencias físicas.
            if (_engine == null ||
                rearLeftWheelCollider == null ||
                rearRightWheelCollider == null)
            {
                Debug.LogError(
                    "CarBehavior: falta el motor o uno de los WheelCollider traseros.",
                    this);
                return;
            }

            // Determinar si corresponde registrar este paso.
            bool shouldLog = false;

            if (!combustionEnabled)
            {
                _powertrainDebugCounter++;

                if (_powertrainDebugCounter >= 5)
                {
                    _powertrainDebugCounter = 0;
                    shouldLog = true;
                }
            }

            // --------------------------------------------------
            // 1. CAPTURAR ESTADO ANTERIOR A LA SIMULACIÓN
            // --------------------------------------------------

            float engineOmegaBefore =
                _engine.FlyWheelAngularVelocity;

            float leftRpmBefore =
                rearLeftWheelCollider.rpm;

            float rightRpmBefore =
                rearRightWheelCollider.rpm;

            float leftTorqueBefore =
                rearLeftWheelCollider.motorTorque;

            float rightTorqueBefore =
                rearRightWheelCollider.motorTorque;

            float gearRatio = GetGearNetRatio();
            float clutchInput = _inputClutch;

            float averageWheelRpm =
                (leftRpmBefore + rightRpmBefore) * 0.5f;

            float transmissionOmegaBefore =
                averageWheelRpm *
                RpmToRadiansPerSecond *
                gearRatio;

            // --------------------------------------------------
            // 2. CAPTURAR INFORMACIÓN DE LA FÍSICA ANTERIOR
            // --------------------------------------------------

            Rigidbody body = null;

            Transform vehicleTransform;

            string leftSlipBefore = "";
            string rightSlipBefore = "";

            Vector3 carForward = Vector3.zero;
            Vector3 carUp = Vector3.zero;

            float leftForwardDot = 0f;
            float rightForwardDot = 0f;
            float leftUpDot = 0f;
            float rightUpDot = 0f;

            string bodyMotion = "";

            if (shouldLog)
            {
                body = GetComponentInParent<Rigidbody>();

                if (body == null)
                    body = GetComponentInChildren<Rigidbody>();

                vehicleTransform =
                    body != null ? body.transform : transform;

                // Capturar el deslizamiento antes de cambiar motorTorque.
                leftSlipBefore = GetWheelSlipInfo(
                    rearLeftWheelCollider,
                    vehicleTransform);

                rightSlipBefore = GetWheelSlipInfo(
                    rearRightWheelCollider,
                    vehicleTransform);

                // Orientación de los colliders.
                carForward = vehicleTransform.forward;
                carUp = vehicleTransform.up;

                leftForwardDot = Vector3.Dot(
                    carForward,
                    rearLeftWheelCollider.transform.forward);

                rightForwardDot = Vector3.Dot(
                    carForward,
                    rearRightWheelCollider.transform.forward);

                leftUpDot = Vector3.Dot(
                    carUp,
                    rearLeftWheelCollider.transform.up);

                rightUpDot = Vector3.Dot(
                    carUp,
                    rearRightWheelCollider.transform.up);

                bodyMotion = body != null
                    ? $"Linear velocity: {body.linearVelocity} | " +
                      $"Angular velocity: {body.angularVelocity}"
                    : "Rigidbody no encontrado";
            }

            // --------------------------------------------------
            // 3. CALCULAR TORQUE DEL EMBRAGUE E INTEGRAR MOTOR
            // --------------------------------------------------

            float clutchTorque = AdvanceEngine(
                accelerationInput,
                additionalTorque,
                combustionEnabled,
                deltaTime);

            // --------------------------------------------------
            // 4. APLICAR TORQUE A LAS RUEDAS
            // --------------------------------------------------

            float totalWheelTorque =
                clutchTorque * gearRatio;

            SetWheelTorque(totalWheelTorque);

            // --------------------------------------------------
            // 5. REGISTRAR RESULTADOS
            // --------------------------------------------------

            if (!shouldLog)
                return;

            Debug.Log(
                $"PREVIOUS PHYSICS | " +
                $"RPM L/R: {leftRpmBefore:F2} / {rightRpmBefore:F2} | " +
                $"Engine omega: {engineOmegaBefore:F2} | " +
                $"Transmission omega: {transmissionOmegaBefore:F2} | " +
                $"Previous MotorTorque L/R: " +
                $"{leftTorqueBefore:F2} / {rightTorqueBefore:F2} | " +
                $"Clutch input: {clutchInput:F2}",
                this);

            Debug.Log(
                $"NEW TORQUE COMMAND | " +
                $"Clutch torque: {clutchTorque:F3} Nm | " +
                $"Wheel torque total: {totalWheelTorque:F3} Nm | " +
                $"New MotorTorque L/R: " +
                $"{rearLeftWheelCollider.motorTorque:F3} / " +
                $"{rearRightWheelCollider.motorTorque:F3} Nm | " +
                $"Gear ratio: {gearRatio:F3}",
                this);

            Debug.Log(
                $"PREVIOUS WHEEL SLIP | LEFT: {leftSlipBefore} | " +
                $"RIGHT: {rightSlipBefore}",
                this);

            Debug.Log(
                $"WHEEL AXES | " +
                $"Car forward: {carForward} | " +
                $"Car up: {carUp} | " +
                $"L forward dot: {leftForwardDot:F2} | " +
                $"R forward dot: {rightForwardDot:F2} | " +
                $"L up dot: {leftUpDot:F2} | " +
                $"R up dot: {rightUpDot:F2}",
                this);

            Debug.Log(
                $"BODY MOTION | {bodyMotion}",
                this);
        }

        /// <summary>
        /// Integra la dinámica angular del motor. Devuelve el torque de embrague calculado
        /// antes de integrar, para que ese mismo valor se aplique al lado de las ruedas.
        /// </summary>
        private float AdvanceEngine(
            float accelerationInput,
            float additionalTorque,
            bool combustionEnabled,
            float deltaTime)
        {
            float clutchTorque = GetLoadTorque();
            float previousOmega = _engine.FlyWheelAngularVelocity;

            float engineTorque = combustionEnabled ? GetCombustionTorque(accelerationInput) : 0f;
            float stateTorque = additionalTorque;

            // La decisión de apagar el motor viene del estado; la resistencia mecánica se calcula aquí.
            if (!combustionEnabled)
                stateTorque -= previousOmega * engineOffDragCoefficient;

            float netTorque = engineTorque
                              + stateTorque
                              - GetFrictionTorque()
                              - clutchTorque;

            if (_engine.inertia <= 0f)
            {
                Debug.LogError("CarEngine.inertia debe ser mayor que cero. Se omitió el paso del motor.", this);
                return clutchTorque;
            }

            float angularAcceleration = netTorque / _engine.inertia;
            float nextOmega = previousOmega + angularAcceleration * deltaTime;

            // Un motor convencional no continúa integrándose hacia una velocidad de giro negativa.
            // No se impone un mínimo artificial de RPM ni el antiguo límite fijo de 1000 rad/s.
            //nextOmega = Mathf.Max(0f, nextOmega);
            if (combustionEnabled)
            {
                nextOmega = Mathf.Max(0f, nextOmega);
            }

            if (float.IsNaN(nextOmega) || float.IsInfinity(nextOmega))
            {
                Debug.LogError("La simulación del motor produjo una velocidad angular no finita.", this);
                return clutchTorque;
            }

            // Integración trapezoidal del ángulo, más precisa que usar solo la velocidad inicial.
            _engine.EnginePhase += (previousOmega + nextOmega) * 0.5f * deltaTime;
            _engine.FlyWheelAngularVelocity = nextOmega;

            SetRPM(_engine.CurrentRPM);
            
            return clutchTorque;

        }

        public void SetWheelTorque(float torque)
        {
            // El torque de transmisión se calcula una sola vez por paso; no se vuelve a suavizar
            // aquí porque ese retraso desincronizaba la reacción del motor y la de las ruedas.
            float torquePerWheel = torque * 0.5f;
            rearLeftWheelCollider.motorTorque = torquePerWheel;
            rearRightWheelCollider.motorTorque = torquePerWheel;
        }

        internal void SetInputBrake(float inputBrake)
        {
            _inputBrake = inputBrake;
        }

        internal void SetInputClutch(float inputClutch)
        {
            if (!autoShift)
            {
                _inputClutch = 1 - inputClutch;
            }
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

                DOTween.To(() => _inputClutch, x => _inputClutch = x, 1f, 0.5f)
                    .SetEase(Ease.OutQuad)
                    .SetUpdate(UpdateType.Fixed)
                    .OnComplete(() =>
                    {
                        Debug.Log("Embrague Liberado");
                    });
            }
            else if (_inputClutch < 0.75f)
            {
                _engine.CurrentGear = targetGear;
                ServiceLocator.Instance.GetService<SetGearUseCase>().SetGearValue(_engine.CurrentGear);
            }
            else
            {
                Debug.Log("No se puede cambiar de marcha: el embrague no está lo suficientemente presionado.");
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
        
        public float GetInputAcceleration()
        {
            return _inputAcceleration;
        }
        public float GetInputBrake()
        {
            return _inputBrake;
        }
        public float GetInputClutch()
        {
            return _inputClutch;
        }
        private string GetWheelSlipInfo(
    WheelCollider wheel,
    Transform vehicleTransform)
        {
            if (!wheel.GetGroundHit(out WheelHit hit))
                return $"{wheel.name}: NoGroundHit";

            WheelFrictionCurve forward = wheel.forwardFriction;
            WheelFrictionCurve sideways = wheel.sidewaysFriction;

            Rigidbody body = wheel.attachedRigidbody;

            Vector3 contactVelocity = body != null
                ? body.GetPointVelocity(hit.point)
                : Vector3.zero;

            float longitudinalSpeed =
                Vector3.Dot(contactVelocity, hit.forwardDir);

            return
                $"Wheel: {wheel.name} | " +
                $"RPM: {wheel.rpm:F2} | " +
                $"MotorTorque: {wheel.motorTorque:F2} | " +
                $"BrakeTorque: {wheel.brakeTorque:F2} | " +
                $"ForwardSlip: {hit.forwardSlip:F3} | " +
                $"SidewaysSlip: {hit.sidewaysSlip:F3} | " +
                $"ContactSpeed: {longitudinalSpeed:F3} | " +
                $"ForwardDir: {hit.forwardDir} | " +
                $"GroundNormal: {hit.normal} | " +
                $"GroundForce: {hit.force:F2} | " +
                $"Ground: {hit.collider.name} | " +
                $"WheelDamping: {wheel.wheelDampingRate:F2} | " +
                $"ForwardFriction: " +
                $"[{forward.extremumSlip:F2}, {forward.extremumValue:F2}, " +
                $"{forward.asymptoteSlip:F2}, {forward.asymptoteValue:F2}, " +
                $"{forward.stiffness:F2}] | " +
                $"SidewaysFriction: " +
                $"[{sideways.extremumSlip:F2}, {sideways.extremumValue:F2}, " +
                $"{sideways.asymptoteSlip:F2}, {sideways.asymptoteValue:F2}, " +
                $"{sideways.stiffness:F2}]";
        }
    }
}