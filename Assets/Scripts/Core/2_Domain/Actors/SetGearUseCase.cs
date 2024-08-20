using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClownCar.Domain
{
    public class SetGearUseCase : SetGear
    {
        private readonly PlayerGearOutput _gearOutput;


        public SetGearUseCase(PlayerGearOutput output)
        {
            _gearOutput = output;
        }

        public void SetGearValue(int value)
        {
            _gearOutput.ShowGear(value);
        }


        
    }
}
