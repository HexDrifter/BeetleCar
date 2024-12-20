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
using JetBrains.Annotations;
using NUnit.Framework.Internal;

public class CarTests : InputTestFixture
{
    private PlayerGameplayUIView _playerGameplayUIView;
    private GameObject _character = Resources.Load<GameObject>("Prefabs/Volkswagen Beetle");
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

    public void initializeTest()
    {
        TestSetup.instance.Init();
        _carHandler = TestSetup.instance._carHandler;
        _gameInput = TestSetup.instance._gameInput;
        _gameInput.Enable();
    }

    [Test]
    public void TestCarInstantiation()
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
        yield return null;
        Release(_gamepad.buttonNorth);
        Set(_gamepad.rightTrigger,.5f);
        yield return new WaitForSeconds(0.2f);
        Press(_gamepad.buttonWest);
        yield return null;
        Release(_gamepad.buttonWest);

        yield return new WaitForSeconds(4f);
        PressAndRelease(_gamepad.buttonWest);

        yield return new WaitForSeconds(4f);
        Assert.Greater(carInstance.transform.position.z, 4f);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestCarSteering()
    {
        GameObject carInstance = GameObject.Instantiate(_character, new Vector3(0f, 1f, 0f), Quaternion.identity);
        TestSetup.instance.Init();
        _carHandler = TestSetup.instance._carHandler;
        _gameInput = TestSetup.instance._gameInput;
        _gameInput.Enable();

        yield return null;
        Press(_gamepad.buttonNorth);
        yield return null;
        Release(_gamepad.buttonNorth);
        Set(_gamepad.rightTrigger, .7f);
        yield return new WaitForSeconds(0.2f);
        Press(_gamepad.buttonWest);
        yield return null;
        Release(_gamepad.buttonWest);
        yield return new WaitForSeconds(1f);
        Set(_gamepad.leftStick, new Vector2(.5f,0f));
        yield return new WaitForSeconds(5f);
        Set(_gamepad.rightTrigger, 0f);
        Set(_gamepad.leftStick, new Vector2(0f,0f));
        Set(_gamepad.leftTrigger, 1f);
        yield return new WaitForSeconds(1f);
        Assert.Greater(carInstance.transform.position.x, 2f);
        yield return null;

    }
    [UnityTest]
    public IEnumerator TestCarIgnition()
    {
        GameObject carInstance = GameObject.Instantiate(_character, new Vector3(0f, 1f, 0f), Quaternion.identity);
        initializeTest();
        yield return new WaitForSeconds(1f);
        PressAndRelease(_gamepad.buttonNorth);
        yield return new WaitForSeconds(1f);
        Assert.Greater(_carHandler.CarBehavior.GetRPM(), 400f);
        yield return null;
    }
    [UnityTest]
    public IEnumerator TestCarEngineThrottle()
    {
        GameObject carInstance = GameObject.Instantiate(_character, new Vector3(0f, 1f, 0f), Quaternion.identity);
        initializeTest();
        yield return new WaitForSeconds(1f);
        PressAndRelease(_gamepad.buttonNorth);
        yield return new WaitForSeconds(1f);
        Assert.Greater(_carHandler.CarBehavior.GetRPM(),400f);
        Set(_gamepad.rightTrigger, 1f);
        yield return new WaitForSeconds(5f);
        Assert.Greater(_carHandler.CarBehavior.GetRPM(), 5000f);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestCarShifting()
    {
        GameObject carInstance = GameObject.Instantiate(_character, new Vector3(0f, 1f, 0f), Quaternion.identity);
        initializeTest();
        yield return new WaitForSeconds(1f);
        PressAndRelease(_gamepad.buttonNorth);
        yield return new WaitForSeconds(1f);
        Assert.AreEqual(_carHandler.CarBehavior.currentGear, 1);
        Set(_gamepad.rightTrigger, .5f);
        yield return new WaitForSeconds(1f);
        PressAndRelease(_gamepad.buttonSouth);
        yield return null;
        Assert.AreEqual(_carHandler.CarBehavior.currentGear, 0);
        yield return new WaitForSeconds(1f);
        PressAndRelease(_gamepad.buttonWest);
        yield return null;
        PressAndRelease(_gamepad.buttonWest);
        yield return null;
        

        Assert.AreEqual(_carHandler.CarBehavior.currentGear, 2);
        PressAndRelease(_gamepad.buttonWest);
        yield return new WaitForSeconds(1f);
        Assert.AreEqual(_carHandler.CarBehavior.currentGear, 3);
        PressAndRelease(_gamepad.buttonWest);
        yield return new WaitForSeconds(1f);
        Assert.AreEqual(_carHandler.CarBehavior.currentGear, 4);
        PressAndRelease(_gamepad.buttonWest);
        yield return new WaitForSeconds(1f);
        Assert.AreEqual(_carHandler.CarBehavior.currentGear, 5);
        yield return new WaitForSeconds(1f);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestCarAccelerationOnFirstGear()
    {
        GameObject carInstance = GameObject.Instantiate(_character, new Vector3(0f, 1f, 0f), Quaternion.identity);
        initializeTest();
        yield return new WaitForSeconds(1f);
        PressAndRelease(_gamepad.buttonNorth);
        yield return new WaitForSeconds(1f);
        Assert.Greater(_carHandler.CarBehavior.GetRPM(), 400f);
        Set(_gamepad.rightTrigger, 1f);
        yield return new WaitForSeconds(.5f);
        PressAndRelease(_gamepad.buttonWest);
        yield return new WaitForSeconds(10f);
        Assert.Greater(_carHandler.CarBehavior.GetRPM(), 5000f);
        Assert.Greater(_carHandler.CarBehavior.GetCarSpeed(), 45f);
        yield return null;
    }

    [UnityTest]
    public IEnumerator TestCarAccelerationTopSpeed()
    {
        GameObject carInstance = GameObject.Instantiate(_character, new Vector3(0f, 1f, -490f), Quaternion.identity);
        initializeTest();
        yield return new WaitForSeconds(1f);
        PressAndRelease(_gamepad.buttonNorth);
        yield return new WaitForSeconds(1f);
        Assert.Greater(_carHandler.CarBehavior.GetRPM(), 400f);
        Set(_gamepad.rightTrigger, 1f);
        yield return null;
        PressAndRelease(_gamepad.buttonWest);
        yield return new WaitForSeconds(8f);
        PressAndRelease(_gamepad.buttonWest);
        yield return new WaitForSeconds(12f);
        PressAndRelease(_gamepad.buttonWest);
        yield return new WaitForSeconds(12f);
        PressAndRelease(_gamepad.buttonWest);
        yield return new WaitForSeconds(10f);
        Assert.Greater(_carHandler.CarBehavior.GetCarSpeed(), 70f);
        yield return null;

    }


    [UnityTest]
    public IEnumerator TestCarBrake()
    {
        GameObject carInstance = GameObject.Instantiate(_character, new Vector3(0f, 1f, 0f), Quaternion.identity);
        initializeTest();
        yield return new WaitForSeconds(1f);
        PressAndRelease(_gamepad.buttonNorth);
        yield return new WaitForSeconds(1f);
        Assert.Greater(_carHandler.CarBehavior.GetRPM(), 400f);
        Set(_gamepad.rightTrigger, 1f);
        yield return new WaitForSeconds(.5f);
        PressAndRelease(_gamepad.buttonWest);
        yield return new WaitForSeconds(10f);
        Set(_gamepad.rightTrigger, 0f);
        Set(_gamepad.leftTrigger, 1f);
        yield return null;
        PressAndRelease(_gamepad.buttonSouth);
        yield return new WaitForSeconds(5f);
        Assert.LessOrEqual(_carHandler.CarBehavior.GetCarSpeed(), 0.1f);
    }
    [TearDown]
    public void TestTearDown()
    {
        if (TestSetup.instance != null)
        {
            TestSetup.instance.TearDown();

        }

    }
}