using ClownCar.Entities;
using ClownCar.Domain;
using ClownCar.Framework;
using ClownCar.InterfaceAdapters;
using ClownCar.SystemUtilities;
using UniRx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Setup : MonoBehaviour
{
    [SerializeField] private PlayerGameplayUIView _playerGameplayUIView;




    private GameInput _gameInput;
    private CarHandler _carHandler;


    void Awake()
    {
        _carHandler = new CarHandler();
        var player = _carHandler.CarBehavior;
        var playerViewModel = new PlayerViewModel(player.engineRPM,player.currentGear);
        var playerPresenter = new PlayerPresenter(playerViewModel);
        var setRpmUseCase = new SetRpmUseCase(playerPresenter);
        _playerGameplayUIView.SetModel(playerViewModel);

        ServiceLocator.Instance.RegisterService<SetRpmUseCase>(setRpmUseCase);

        var setGearUseCase = new SetGearUseCase(playerPresenter);

        ServiceLocator.Instance.RegisterService<SetGearUseCase>(setGearUseCase);
        
        _gameInput = new GameInput();
        _gameInput.Enable();

    }

    void Update()
    {
        _carHandler.SetInputDirection(_gameInput.Actor.Direction.ReadValue<Vector2>());
        _carHandler.SetInputAcceleration(_gameInput.Actor.Acceleration.ReadValue<float>());
        _carHandler.SetInputBrake(_gameInput.Actor.Brake.ReadValue<float>());
        _carHandler.SetInputGearUp(_gameInput.Actor.ShiftUp.triggered);
        _carHandler.SetInputGearDown(_gameInput.Actor.ShiftDown.triggered);
        _carHandler.Tick();
    }
}
