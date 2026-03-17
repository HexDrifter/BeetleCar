using Beetle.Framework;
using Beetle.InterfaceAdapters;
using Beetle.Domain;
using Beetle.SystemUtilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSetup : MonoBehaviour
{
    private PlayerGameplayUIView _playerGameplayUIView;
    public CarHandler _carHandler;
    public GameInput _gameInput;
    public static TestSetup instance;
    public CarAnimator _carAnimator;

    private void Awake()
    {
        instance = this;
    }

    public void Init()
    {
        _carAnimator = FindAnyObjectByType<CarAnimator>();
        _gameInput = new GameInput();
        _carHandler = new CarHandler();
        _playerGameplayUIView = GameObject.FindAnyObjectByType<PlayerGameplayUIView>();
        var player = _carHandler.CarBehavior;
        var playerViewModel = new PlayerViewModel(player.Engine.CurrentRPM, player.Engine.CurrentGear, player.carSpeed);
        var playerPresenter = new PlayerPresenter(playerViewModel);
        var setRpmUseCase = new SetRpmUseCase(playerPresenter);
        _playerGameplayUIView.SetModel(playerViewModel);
        ServiceLocator.Instance.RegisterService<SetRpmUseCase>(setRpmUseCase);
        var setGearUseCase = new SetGearUseCase(playerPresenter);
        ServiceLocator.Instance.RegisterService<SetGearUseCase>(setGearUseCase);
        var setSpeedUseCase = new SetSpeedUseCase(playerPresenter);
        ServiceLocator.Instance.RegisterService<SetSpeedUseCase>(setSpeedUseCase);
        _carHandler.CarBehavior.Initialize();
    }
    void Update()
    {
        if (_carHandler != null)
        {
            _carHandler.SetInputDirection(_gameInput.Actor.Direction.ReadValue<Vector2>());
            _carHandler.SetInputAcceleration(_gameInput.Actor.Acceleration.ReadValue<float>());
            _carHandler.SetInputBrake(_gameInput.Actor.Brake.ReadValue<float>());
            _carHandler.SetInputGearUp(_gameInput.Actor.ShiftUp.triggered);
            _carHandler.SetInputGearDown(_gameInput.Actor.ShiftDown.triggered);
            _carHandler.SetEngineIgnition(_gameInput.Actor.StartEngine.triggered);
            _carHandler.Tick();
        }

    }
    public void TearDown()
    {
        try
        { 
            ServiceLocator.Instance.RemoveService<SetRpmUseCase>();
            ServiceLocator.Instance.RemoveService<SetGearUseCase>();
            ServiceLocator.Instance.RemoveService<SetSpeedUseCase>();
        }
        catch { }
    }
}
