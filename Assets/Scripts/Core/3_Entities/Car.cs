using UnityEngine;

namespace Beetle.Entities
{
    public class Car
    {
        public float EngineRPM { get; private set; }
        public int CurrentGear { get; private set; }
        public float[] GearRatios { get; }
        public float FinalDriveRatio { get; }
        public float MinRPM { get; }
        public float MaxRPM { get; }
        private float _inertia;
        private float _torque;

        public Car(float[] gearRatios, float finalDriveRatio, float minRPM, float maxRPM, float inertia)
        {
            GearRatios = gearRatios;
            FinalDriveRatio = finalDriveRatio;
            MinRPM = minRPM;
            MaxRPM = maxRPM;
            _inertia = inertia;
            CurrentGear = 1;
            EngineRPM = minRPM;
        }

        public void SetEngineRPM(float throttleInput, float gearRatio, float finalDriveRatio, float currentWheelRPM, int currentGear)
        {
            // Configuración del torque del motor
            _torque = (throttleInput * _inertia * 1000f) - 50f;

            // Cálculo de las RPM del motor basadas en la relación de cambio (gear ratio)
            if (gearRatio != 0f)
            {
                EngineRPM = currentWheelRPM * (gearRatio * finalDriveRatio);
            }
            else
            {
                // En caso de que el vehículo esté en neutral, las RPM del motor siguen la entrada de aceleración directamente
                EngineRPM = Mathf.Clamp(EngineRPM + _torque, 0f, MaxRPM);
            }

            // Control de la velocidad de las RPM del motor según el cambio de marcha
            // Aceleración y desaceleración
            if (currentGear != 1) // Solo si no está en neutral
            {
                // Aplicar el torque de la aceleración, pero clamped por el máximo de RPM
                EngineRPM = Mathf.Clamp(EngineRPM + _torque, 0f, MaxRPM);
            }
            else // En neutral, el motor sigue acelerando de acuerdo al input
            {
                // Aceleración en neutral
                EngineRPM = Mathf.Clamp(EngineRPM + _torque, 0f, MaxRPM);
            }
        }

        public float GetEngineRPM()
        {
            return EngineRPM;
        }

        public float GetWheelTorque()
        {
            return _torque * GearRatios[CurrentGear] * FinalDriveRatio;
        }
    }
}
