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

        // El estado decide cuánto acelerador necesita el ralentí. Los torques mecánicos se
        // consultan a CarBehavior; el cálculo del giro y la transmisión sigue centralizado allí.
        float requiredTorque = Owner.GetFrictionTorque() + Owner.GetLoadTorque();
        float idleCorrectionTorque = idleError * 0.2f;
        requiredTorque = Mathf.Max(0f, requiredTorque + idleCorrectionTorque);

        float fullThrottleTorque = Owner.GetCombustionTorque(1f);
        float idleThrottle = fullThrottleTorque > 0.01f
            ? Mathf.Clamp01(requiredTorque / fullThrottleTorque)
            : 0f;

        Owner.SimulatePowertrain(idleThrottle);
        

    }

}
