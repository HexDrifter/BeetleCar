using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace Beetle.InterfaceAdapters
{
    public class PlayerViewModel
    {
        public FloatReactiveProperty Rpm;
        public FloatReactiveProperty Speed;
        public IntReactiveProperty Gear;

        public PlayerViewModel(float rpm, int gear, float speed)
        {
            Rpm = new FloatReactiveProperty(rpm);
            Speed = new FloatReactiveProperty(speed);
            Gear = new IntReactiveProperty(gear);
        }
    }

}
