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

public class InputTests : InputTestFixture
{
    private Keyboard    _keyboard;
    private Gamepad     _gamepad;
    private GameInput   _gameInput;

    public override void Setup()
    {
        SceneManager.LoadScene("Scenes/TestingRoom");
        base.Setup();
        _keyboard = InputSystem.AddDevice<Keyboard>();
        _gamepad = InputSystem.AddDevice<Gamepad>();
        _gameInput = new GameInput();
        var mouse = InputSystem.AddDevice<Mouse>();

        Press(mouse.rightButton);
        Release(mouse.rightButton);
    }

    [Test]
    public void TestInputButton()
    {
        _gameInput.Enable();
        Press(_gamepad.buttonSouth);
        Assert.IsTrue(_gamepad.buttonSouth.IsPressed());
        Debug.Log(_gamepad.buttonSouth.ReadValue());
        Assert.IsTrue(_gameInput.Actor.ShiftDown.IsPressed());
    }
    

    [Test]
    public void TestInputTrigger()
    {
        _gameInput.Enable();
        Set(_gamepad.rightTrigger, 1f);
        Debug.Log(_gamepad.rightShoulder.value);
        Debug.Log(_gamepad.rightTrigger.value);
        Debug.Log(_gameInput.Actor.Acceleration.IsPressed());
        Debug.Log(_gameInput.Actor.Acceleration.ReadValue<float>());
        Assert.IsTrue( _gameInput.Actor.Acceleration.IsPressed() );
        Assert.GreaterOrEqual(_gameInput.Actor.Acceleration.ReadValue<float>(), 1f);
    }
}
