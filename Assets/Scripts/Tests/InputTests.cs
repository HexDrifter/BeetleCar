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
        Press(_keyboard.downArrowKey);
        Assert.IsTrue(_keyboard.downArrowKey.IsPressed());
        Debug.Log(_keyboard.downArrowKey.ReadValue());
        Assert.IsTrue(_gameInput.Actor.TestButton.IsPressed());
    }
    [UnityTest]
    public IEnumerator TestInputRepeatedButton()
    {
        _gameInput.Enable();
        Press(_keyboard.downArrowKey);
        yield return null;
        Assert.IsTrue(_keyboard.downArrowKey.IsPressed());
        Debug.Log(_keyboard.downArrowKey.ReadValue());
        Assert.IsTrue(_gameInput.Actor.TestButton.IsPressed());
        Release(_keyboard.downArrowKey);
        yield return new WaitForSeconds(2f);
        Press(_keyboard.downArrowKey);
        yield return null;
        Assert.IsTrue(_keyboard.downArrowKey.IsPressed());
        Debug.Log(_keyboard.downArrowKey.ReadValue());
        Assert.IsTrue(_gameInput.Actor.TestButton.IsPressed());
        Release(_keyboard.downArrowKey);
        yield return new WaitForSeconds(2f);
        Press(_keyboard.downArrowKey);
        yield return null;
        Assert.IsTrue(_keyboard.downArrowKey.IsPressed());
        Debug.Log(_keyboard.downArrowKey.ReadValue());
        Assert.IsTrue(_gameInput.Actor.TestButton.IsPressed());
        Release(_keyboard.downArrowKey);
        yield return new WaitForSeconds(2f);
    }

    [Test]
    public void TestInputTrigger()
    {
        _gameInput.Enable();
        Set(_gamepad.rightShoulder, 1f);
        Set(_gamepad.rightTrigger, 1f);
        Debug.Log(_gamepad.rightShoulder.value);
        Debug.Log(_gamepad.rightTrigger.value);
        Debug.Log(_gameInput.Actor.TestTrigger.IsPressed());
        Debug.Log(_gameInput.Actor.TestTrigger.ReadValue<float>());
        Assert.IsTrue( _gameInput.Actor.TestTrigger.IsPressed() );
        Assert.GreaterOrEqual(_gameInput.Actor.TestTrigger.ReadValue<float>(), 1f);
    }
}
