using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClownCar.Framework
{

    public class CarHandler
    {
        private Vector2 _inputDirection;
        private float _inputAcceleration;
        private float _inputBrake;
    
        private CarBehavior _carBehavior;


        public CarBehavior CarBehavior => _carBehavior;

        public CarHandler()
        {
            _carBehavior = GameObject.FindGameObjectWithTag("Player").GetComponent<CarBehavior>();
        }

        public void SetInputDirection(Vector2 inputDirection)
        {
            _inputDirection = inputDirection;
        }

        public void SetInputAcceleration(float inputAcceleration)
        {
            _inputAcceleration = inputAcceleration;
        }

        public void SetInputBrake(float inputBrake)
        {
            _inputBrake = inputBrake;
        }

        public void Tick()
        {
            _carBehavior.SetInputDirection(_inputDirection);
            _carBehavior.SetInputAcceleration(_inputAcceleration);
            _carBehavior.SetInputBrake(_inputBrake);
            _carBehavior.tick();
        }

        public void SetInputGearUp(bool shifting)
        {
            if (shifting)
            {
                _carBehavior.ShiftUp();
            }
        }

        public void SetInputGearDown(bool shifting)
        {
            if (shifting)
            {
                _carBehavior.ShiftDown();
            }
        }
    }

}