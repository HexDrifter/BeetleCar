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
        
        Owner.SetEngineOmega(Owner.inputAcceleration);
        float wheelTorque = Owner.GetLoadTorque() * Owner.GetGearNetRatio();
        Owner.SetWheelTorque(wheelTorque);

    }
}
