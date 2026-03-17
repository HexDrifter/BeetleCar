using UnityEngine;
using Beetle.Framework;
public class CarStartingState : CarEngineState
{
    private float _starterTorque = 10f;
    public CarStartingState(CarBehavior owner) : base(owner)
    {
    }
    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Arrancando");
        Owner.SetEngineOmega(0f, 100f);
    }
    public override void Tick()
    {
        Owner.SetEngineOmega(0f, _starterTorque);
        float wheelTorque = Owner.GetLoadTorque() * Owner.GetGearNetRatio();
        Owner.SetWheelTorque(wheelTorque);


    }
}
