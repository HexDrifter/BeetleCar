using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beetle.Domain
{
    using Beetle.Entities;
    public class SetSpeedUseCase : SetSpeed
    {
        private readonly PlayerSpeedOutput _playerSpeedOutput;

        public SetSpeedUseCase(PlayerSpeedOutput playerSpeedOutput)
        {
            _playerSpeedOutput = playerSpeedOutput;
        }

        public void SetSpeedValue(float value)
        {
            _playerSpeedOutput.ShowSpeed(value);
        }
    }
}
