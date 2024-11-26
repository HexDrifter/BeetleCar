using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Beetle.Framework
{

    public class CarHandler
    {
        private Vector2 _inputDirection;
        private float _inputAcceleration;
        private float _inputBrake;
        private bool _inputEngineIgnition;
    
        private CarBehavior _carBehavior;


        public CarBehavior CarBehavior => _carBehavior;

        public CarHandler()
        {
            
            _carBehavior = GameObject.FindGameObjectWithTag("Player").GetComponent<CarBehavior>();
            //_carBehavior.Initialize();
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

        public void SetEngineIgnition(bool ignition)
        {
            _inputEngineIgnition = ignition;
        }

        public void Tick()
        {
            _carBehavior.SetInputDirection(_inputDirection);
            _carBehavior.SetInputAcceleration(_inputAcceleration);
            _carBehavior.SetInputBrake(_inputBrake);
            _carBehavior.SetInputEngineIgnition(_inputEngineIgnition);
            _carBehavior.Tick();
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