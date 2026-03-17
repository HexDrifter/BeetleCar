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

        Owner.SetEngineOmega(0f);
        float wheelTorque = Owner.GetLoadTorque() * Owner.GetGearNetRatio();
        Owner.SetWheelTorque(wheelTorque);

    }
}
