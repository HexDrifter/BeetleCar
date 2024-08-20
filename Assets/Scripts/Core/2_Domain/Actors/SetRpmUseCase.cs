using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ClownCar.Domain
{
    using ClownCar.Entities;
    public class SetRpmUseCase : SetRpm
    {
        private readonly PlayerRpmOutput _rpmOutput;

        public SetRpmUseCase(PlayerRpmOutput output)
        {
            _rpmOutput = output;
        }

        public void SetRpmValue(float value)
        {
            _rpmOutput.ShowRPM(value);
        }
    }
}
