using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


namespace ClownCar.InterfaceAdapters
{
    public class PlayerViewModel
    {
        public FloatReactiveProperty Rpm;
        public IntReactiveProperty Gear;

        public PlayerViewModel(float rpm, int gear)
        {

            Rpm = new FloatReactiveProperty(rpm);
            Gear = new IntReactiveProperty(gear);
            Debug.Log(Rpm);
        }
    }

}
