using UnityEngine;
using Beetle.Framework;

public class CarEngineOffState : CarEngineState
{
    public CarEngineOffState(CarBehavior owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Motor Apagado");
    }

    public override void Tick()
    {
        base.Tick();
        float compressionResistance = Owner.Engine.FlyWheelAngularVelocity * 3f;
        Owner.SetEngineOmega(0f, -compressionResistance);

        float wheelTorque = Owner.GetLoadTorque() * Owner.GetGearNetRatio();
        Owner.SetWheelTorque(wheelTorque);

    }

    public override void OnExit()
    {
        base.OnExit();
        //Owner.SetRPM(450);
    }
}
