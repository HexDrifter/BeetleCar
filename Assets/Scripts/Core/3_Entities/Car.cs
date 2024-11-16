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
            // Configuraci�n del torque del motor
            _torque = (throttleInput * _inertia * 1000f) - 50f;

            // C�lculo de las RPM del motor basadas en la relaci�n de cambio (gear ratio)
            if (gearRatio != 0f)
            {
                EngineRPM = currentWheelRPM * (gearRatio * finalDriveRatio);
            }
            else
            {
                // En caso de que el veh�culo est� en neutral, las RPM del motor siguen la entrada de aceleraci�n directamente
                EngineRPM = Mathf.Clamp(EngineRPM + _torque, 0f, MaxRPM);
            }

            // Control de la velocidad de las RPM del motor seg�n el cambio de marcha
            // Aceleraci�n y desaceleraci�n
            if (currentGear != 1) // Solo si no est� en neutral
            {
                // Aplicar el torque de la aceleraci�n, pero clamped por el m�ximo de RPM
                EngineRPM = Mathf.Clamp(EngineRPM + _torque, 0f, MaxRPM);
            }
            else // En neutral, el motor sigue acelerando de acuerdo al input
            {
                // Aceleraci�n en neutral
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
