using UnityEngine;
using Beetle.Framework;

public class CarIdleState : CarEngineState
{
    public CarIdleState(CarBehavior owner) : base(owner)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Ralentí");
        //Owner.SetRPM(450);
    }

    public override void Tick()
    {
        base.Tick();

        float idleError = Owner.Engine.MinRPM - Owner.Engine.CurrentRPM;
        float idleTorque = 0f;

        if (idleError > 0)
        {
            idleTorque = Mathf.Min(50f, idleError * 0.2f);
        }

        Owner.SetEngineOmegaFromIdle(idleTorque);

        float wheelTorque = Owner.GetLoadTorque() * Owner.GetGearNetRatio();
        Owner.SetWheelTorque(wheelTorque);
        

    }

}
