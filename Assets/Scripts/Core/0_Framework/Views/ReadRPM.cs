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
    public CarBehavior car;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rpmText.text = "RPM:" + car.Engine.CurrentRPM;
        differentialOmega.text = "Differential Omega:" + car.GetWheelAngularVelocity(car.rearLeftWheelCollider);
        EngineTarget.text = "Engine Target Velocity:\nAngular:" +
            car.GetEngineTargetVelocity() + "\nRPM:" +
            car.GetEngineTargetVelocity() * 9.5493f;

    }
}
