using Beetle.Framework;
using TMPro;
using UnityEngine;

public class DebugCar : MonoBehaviour
{
    public TextMeshProUGUI rpmText;
    public TextMeshProUGUI engineTargetVelocity;
    public TextMeshProUGUI wheelAngularText;
    public CarBehavior car;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rpmText.text = "Current RPM:" + car.Engine.CurrentRPM;
        wheelAngularText.text = "WheelAngular:" + car.GetWheelAngularVelocity(car.rearLeftWheelCollider);
        engineTargetVelocity.text = "Engine Target Velocity:\nAngular:"+
            car.GetEngineTargetVelocity()+"\nRPM:"+
            car.GetEngineTargetVelocity()*9.5493f;
    }
}
