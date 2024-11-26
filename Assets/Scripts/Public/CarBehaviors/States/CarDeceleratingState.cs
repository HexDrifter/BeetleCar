using UnityEngine;
using Beetle.Framework;

public class CarDeceleratingState : CarEngineState
{
    public CarDeceleratingState(CarBehavior owner) : base(owner)
    {
    }
    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Desacelerando");
    }

    public override void Tick()
    {
        base.Tick();

        if (Owner.gearRatios[Owner.currentGear] != 0f)
        {
            float calculatedRPM = Owner.GetWheelRPM() * Owner.GetCurrentGearRatio() * Owner.finalDriveRatio;
            Owner.SetRPM(Mathf.Lerp(
                            Owner.GetRPM(),
                            calculatedRPM,Time.deltaTime * 5f
                            ));
            Owner.SetWheelTorque(-70f);
        }
        else
        {
            Owner.SetRPM(Mathf.Lerp(
                        Owner.engineRPM,
                        0f, Time.deltaTime *2f
                        ));
        }
        
    }
}
