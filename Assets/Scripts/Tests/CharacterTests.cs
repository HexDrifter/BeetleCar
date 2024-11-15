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
    private Gamepad _gamepad;
    private GameInput _gameInput;

    public override void Setup()
    {
        SceneManager.LoadScene("Scenes/TestingRoom");
        base.Setup();
        _keyboard = InputSystem.AddDevice<Keyboard>();
        _gamepad = InputSystem.AddDevice<Gamepad>();
        
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
        TestSetup.instance.Init();
        _carHandler = TestSetup.instance._carHandler;
        _gameInput = TestSetup.instance._gameInput;
        _gameInput.Enable();
        
        
        Press(_gamepad.buttonNorth);
        Press(_keyboard.wKey);
        yield return null;
        Release(_gamepad.buttonNorth);

        yield return new WaitForSeconds(4f);
        Release(_keyboard.wKey);
        Press(_keyboard.sKey);

        yield return new WaitForSeconds(4f);
        Assert.Greater(carInstance.transform.position.z, 4f);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestSteeringCar()
    {
        GameObject carInstance = GameObject.Instantiate(_character, new Vector3(0f, 1f, 0f), Quaternion.identity);
        TestSetup.instance.Init();
        _carHandler = TestSetup.instance._carHandler;
        _gameInput = TestSetup.instance._gameInput;
        _gameInput.Enable();

        yield return null;

        PressAndRelease(_gamepad.buttonNorth);
        Set(_gamepad.rightTrigger, 1f);
        yield return new WaitForSeconds(2f);
        Set(_gamepad.leftStick, new Vector2(.8f,0f));
        yield return new WaitForSeconds(1f);
        Set(_gamepad.rightTrigger, 0f);
        Set(_gamepad.leftStick, new Vector2(0f,0f));
        Set(_gamepad.leftTrigger, 1f);
        yield return new WaitForSeconds(1f);
        Assert.Greater(carInstance.transform.position.x, 2f);
        yield return null;

    }
}