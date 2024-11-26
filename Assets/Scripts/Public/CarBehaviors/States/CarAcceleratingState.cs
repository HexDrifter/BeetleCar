using Beetle.Framework;
using UnityEngine;

public class CarAcceleratingState : CarEngineState
{
    public CarAcceleratingState(CarBehavior owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Acelerando");
        
    }

    public override void Tick()
    {
        base.Tick();
        float torque;
        if (Owner.GetCurrentGearRatio() != 0f)
        {
            float calculatedRPM = Owner.GetWheelRPM() * Owner.GetCurrentGearRatio() * Owner.finalDriveRatio;
            Owner.SetRPM(Mathf.Lerp(
                            Owner.GetRPM(),
                            Mathf.Clamp(calculatedRPM + (Owner.engineTorque * Owner.inputAcceleration),
                                   Owner.minRPM,
                                   Owner.maxRPM),
                                   Time.deltaTime * 5f
                            ));
            torque = Owner.torqueCurve.Evaluate(Owner.GetRPM() / Owner.maxRPM) * Owner.inputAcceleration * Owner.GetCurrentGearRatio();
            
        }
        else
        {
            Owner.SetRPM(Mathf.Clamp(
                        Owner.engineRPM + (Owner.engineTorque * Owner.inputAcceleration * Time.deltaTime * 10),
                        Owner.minRPM, Owner.maxRPM
                        ));
            torque = 0f;
        }
        Owner.SetWheelTorque(torque);
    }
}
