using UnityEngine;
using Beetle.Framework;
using UnityEngine.UI;
using TMPro;
using Beetle.SystemUtilities;
using Beetle.Domain;

public class ReadRPM : MonoBehaviour
{
    public TextMeshProUGUI rpmText;
    public TextMeshProUGUI differentialOmega;
    public TextMeshProUGUI EngineTarget;
    public TextMeshProUGUI loadTorque;
    public TextMeshProUGUI currentTransmissionOmega;

    [Header("Pedals")]
    public Slider accelerationSlider;
    public Slider brakeSlider;
    public Slider ClutchSlider;
    public CarBehavior car;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rpmText.text = "RPM:" + car.Engine.CurrentRPM;
        differentialOmega.text = "Differential Omega:" + car.GetWheelAngularVelocity(car.rearLeftWheelCollider);
        loadTorque.text = "Load Torque:" + car.GetLoadTorque();
        currentTransmissionOmega.text = "Current Transmission Omega:" + car.GetCurrentTransmissionOmega();
        EngineTarget.text = "Engine Target Velocity:\nAngular:" +
            car.GetEngineTargetVelocity() + "\nRPM:" +
            car.GetEngineTargetVelocity() * 9.5493f;
        accelerationSlider.value = car.GetInputAcceleration();
        brakeSlider.value = car.GetInputBrake();
        ClutchSlider.value = car.GetInputClutch();

    }
}
