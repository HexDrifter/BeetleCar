using UnityEngine;

namespace Beetle.Entities
{
    public class CarEngine
    {
        //Datos dinámicos
        public float FlyWheelAngularVelocity { get; set; }
        public float EnginePhase { get; set; }
        public int CurrentGear { get; set; }

        //Datos maestros
        public float inertia { get; private set; }
        public float[] GearRatios { get; private set; }
        public float FinalDriveRatio { get; private set; }
        public float MinRPM { get; private set; }
        public float MaxRPM { get; private set; }

        //Propiedades calculadas
        public float CurrentRPM => FlyWheelAngularVelocity * 9.5493f;
        public int MaxGears => GearRatios.Length -1;

        //Atajo para obtener ratio actual
        public float CurrentGearRatio => GearRatios[CurrentGear];
        public CarEngine(float[] ratios, float finalRatio, float inertiaValue = 0.75f, float minRev = 850f, float maxRev = 6000f)
        {
            GearRatios = ratios;
            FinalDriveRatio = finalRatio;
            inertia = inertiaValue;
            MinRPM = minRev;
            MaxRPM = maxRev;

            CurrentGear = 1;
            FlyWheelAngularVelocity = 0f;
            EnginePhase = 0f;

        }
    }
}
