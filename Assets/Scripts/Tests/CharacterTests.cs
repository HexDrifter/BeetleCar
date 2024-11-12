using Beetle.Entities;
using Beetle.Domain;
using Beetle.Framework;
using Beetle.InterfaceAdapters;
using Beetle.SystemUtilities;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;



public class CharacterTests : InputTestFixture
{
    private PlayerGameplayUIView _playerGameplayUIView;
    private GameObject _character = Resources.Load<GameObject>("Prefabs/Car");
    private CarHandler _carHandler;
    private Keyboard _keyboard;
    private GameInput _gameInput;

    public override void Setup()
    {
        SceneManager.LoadScene("Scenes/TestingRoom");
        base.Setup();
        _keyboard = InputSystem.AddDevice<Keyboard>();
        
        _gameInput = new GameInput();
        var mouse = InputSystem.AddDevice<Mouse>();
        
        Press(mouse.rightButton);
        Release(mouse.rightButton);
    }

    [Test]
    public void TestPlayerInstantiation()
    {
        GameObject characterInstance = GameObject.Instantiate(_character, Vector3.zero, Quaternion.identity);
        Assert.That(characterInstance, !Is.Null);
    }
    [UnityTest]
    public IEnumerator TestCarMoves()
    {
        GameObject carInstance = GameObject.Instantiate(_character, new Vector3(0f, 1f, 0f), Quaternion.identity);
        
        _gameInput.Enable();
        _carHandler = new CarHandler();
        _playerGameplayUIView = GameObject.FindAnyObjectByType<PlayerGameplayUIView>();
        var player = _carHandler.CarBehavior;
        var playerViewModel = new PlayerViewModel(player.engineRPM, player.currentGear);
        var playerPresenter = new PlayerPresenter(playerViewModel);
        var setRpmUseCase = new SetRpmUseCase(playerPresenter);
        _playerGameplayUIView.SetModel(playerViewModel);
        ServiceLocator.Instance.RegisterService<SetRpmUseCase>(setRpmUseCase);
        var setGearUseCase = new SetGearUseCase(playerPresenter); 
        ServiceLocator.Instance.RegisterService<SetGearUseCase>(setGearUseCase);
        Press(_keyboard.shiftKey);
        _carHandler.SetInputGearUp(_gameInput.Actor.ShiftUp.triggered);
        Press(_keyboard.wKey);
        _carHandler.SetInputAcceleration(_gameInput.Actor.Acceleration.ReadValue<float>());
        yield return new WaitForSeconds(2f);
        Release(_keyboard.wKey);
        _carHandler.SetInputAcceleration(_gameInput.Actor.Acceleration.ReadValue<float>());
        yield return new WaitForSeconds(2f);
        Assert.That(carInstance.transform.position.z, Is.GreaterThan(1f));
    }


}