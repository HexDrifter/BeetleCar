using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beetle.Entities
{
    public class Player
    {
        public int gear { get; private set; }
        public float rpm { get; private set; }

        public Player()
        {

        }

        public void SetRpm(float value)
        {
            rpm = value;
        }
        public void SetGear(int value)
        {
            gear = value;
        }
    }
}
