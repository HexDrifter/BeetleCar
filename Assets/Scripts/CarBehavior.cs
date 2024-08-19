using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarBehavior : MonoBehaviour
{
    private Vector2 _inputDirection;
    private float _inputAcceleration;
    private float _inputBrake;


    [Header("Config")]
    [SerializeField] public float engineTorque;
    [SerializeField] public float brakeForce;
    [SerializeField] public float maxAngle;
    
    public Vector2 inputDirection => _inputDirection;
    public float inputAcceleration => _inputAcceleration;
    public float inputBrake => _inputBrake;


    [Header("Wheel Colliders")]
    [SerializeField] public WheelCollider frontLeftWheelCollider;
    [SerializeField] public WheelCollider frontRightWheelCollider;
    [SerializeField] public WheelCollider rearLeftWheelCollider;
    [SerializeField] public WheelCollider rearRightWheelCollider;

    [Header("Wheel Transform")]
    [SerializeField] public Transform frontLeftWheel;
    [SerializeField] public Transform frontRightWheel;
    [SerializeField] public Transform rearLeftWheel;
    [SerializeField] public Transform rearRightWheel;

    [Header("Wheel Calipers")]
    [SerializeField] public Transform frontLeftCaliper;
    [SerializeField] public Transform frontRightCaliper;
    [SerializeField] public Transform rearLeftCaliper;
    [SerializeField] public Transform rearRightCaliper;

    internal void SetInputDirection(Vector2 inputDirection)
    {
        _inputDirection = inputDirection;
    }

    internal void SetCarDirection()
    {
        frontLeftWheelCollider.steerAngle = maxAngle * _inputDirection.x;
        frontRightWheelCollider.steerAngle = maxAngle* _inputDirection.x;
    }
    internal void SetInputAcceleration(float inputAcceleration)
    {
        _inputAcceleration = inputAcceleration;
    }

    internal void SetCarAcceleration()
    {
        rearLeftWheelCollider.motorTorque = engineTorque * _inputAcceleration;
        rearRightWheelCollider.motorTorque = engineTorque * _inputAcceleration;
    }

    internal void SetInputBrake(float inputBrake)
    {
        _inputBrake = inputBrake;
    }

    internal void SetCarBrake()
    {
        frontLeftWheelCollider.brakeTorque = brakeForce * _inputBrake;
        frontRightWheelCollider.brakeTorque = brakeForce * _inputBrake;
        rearLeftWheelCollider.brakeTorque = brakeForce * _inputBrake;
        rearRightWheelCollider.brakeTorque = brakeForce * _inputBrake;
        
    }

    internal void AnimateWheels()
    {
        frontLeftWheelCollider.GetWorldPose(out Vector3 frontLeftWheelPose, out Quaternion frontLeftWheelRotation);
        frontRightWheelCollider.GetWorldPose(out Vector3 frontRightWheelPose, out Quaternion frontRightWheelRotation);

        frontLeftWheelRotation *= Quaternion.Euler(0, -90, 0);
        frontRightWheelRotation *= Quaternion.Euler(0, 90, 0);
        
        frontLeftWheel.transform.rotation = frontLeftWheelRotation;
        frontRightWheel.transform.rotation = frontRightWheelRotation;
        
        frontLeftWheel.transform.position = frontLeftWheelPose;
        frontRightWheel.transform.position = frontRightWheelPose;
        
        frontLeftCaliper.transform.position = frontLeftWheelPose;
        frontLeftCaliper.transform.rotation = Quaternion.Euler(0, frontLeftWheelRotation.eulerAngles.y, 0);

        frontRightCaliper.transform.position = frontRightWheelPose;
        frontRightCaliper.transform.rotation = Quaternion.Euler(0, frontRightWheelRotation.eulerAngles.y, 0);



        rearLeftWheelCollider.GetWorldPose(out Vector3 rearLeftWheelPose, out Quaternion rearLeftWheelRotation);
        rearRightWheelCollider.GetWorldPose(out Vector3 rearRightWheelPose, out Quaternion rearRightWheelRotation);
        
        rearLeftWheelRotation *= Quaternion.Euler(0, -90, 0);
        rearRightWheelRotation *= Quaternion.Euler(0, 90, 0);
        
        rearLeftWheel.transform.rotation = rearLeftWheelRotation;
        rearRightWheel.transform.rotation = rearRightWheelRotation;

        rearLeftWheel.transform.position = rearLeftWheelPose;
        rearRightWheel.transform.position = rearRightWheelPose;

        rearLeftCaliper.transform.position = rearLeftWheelPose;
        rearLeftCaliper.transform.rotation = Quaternion.Euler(0, rearLeftWheelRotation.eulerAngles.y, 0);

        rearRightCaliper.transform.position = rearRightWheelPose;
        rearRightCaliper.transform.rotation = Quaternion.Euler(0, rearRightWheelRotation.eulerAngles.y, 0);
    }

    internal void Update()
    {
        SetCarDirection();
        SetCarAcceleration();
        SetCarBrake();
        AnimateWheels();
    }

    
}
