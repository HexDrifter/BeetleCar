using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Setup : MonoBehaviour
{
    private GameInput _gameInput;
    private CarHandler _carHandler;


    void Awake()
    {
        _carHandler = new CarHandler();
        _gameInput = new GameInput();
        _gameInput.Enable();

    }

    // Update is called once per frame
    void Update()
    {
        _carHandler.SetInputDirection(_gameInput.Actor.Direction.ReadValue<Vector2>());
        _carHandler.SetInputAcceleration(_gameInput.Actor.Acceleration.ReadValue<float>());
        _carHandler.SetInputBrake(_gameInput.Actor.Brake.ReadValue<float>());
        _carHandler.Tick();
    }
}
