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
        Owner.SetRPM(0);
        Owner.SetWheelTorque(0);
    }

    public override void Tick()
    {
        base.Tick();
        Owner.SetRPM(0);
        Owner.SetWheelTorque(0);

    }

    public override void OnExit()
    {
        base.OnExit();
        Owner.SetRPM(450);
    }
}
