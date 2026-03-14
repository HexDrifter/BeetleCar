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

        float idleError = Owner.minRPM - Owner.engineRPM;
        float idleTorque = 0f;

        if (idleError > 0)
        {
            idleTorque = Mathf.Min(50f, idleError * 0.2f);
        }

        Owner.SetEngineOmegaFromIdle(idleTorque);

        float wheelTorque = Owner.GetLoadTorque() * Owner.GetGearNetRatio();
        Owner.SetWheelTorque(wheelTorque);
        /*
        if (Owner.GetRPM() < Owner.minRPM && Owner.GetCurrentGearRatio() != 0)
        {
            float calculatedRPM = Owner.GetWheelRPM() * Owner.GetCurrentGearRatio() * Owner.finalDriveRatio;
            Owner.SetRPM(Mathf.Lerp(
                            Owner.GetRPM(),
                            Mathf.Clamp(calculatedRPM,
                                   0,
                                   Owner.maxRPM),
                                   Time.deltaTime * 5f
                            ));
            float idletorque = 15f * Owner.GetCurrentGearRatio() * Owner.finalDriveRatio;
            Owner.SetWheelTorque(idletorque);
        }
        else
        {
            Owner.SetWheelTorque(0);
            float rpm = Mathf.Lerp(Owner.GetRPM(), Owner.minRPM + Random.Range(-50f, 50f), Time.deltaTime * 2f);
            Owner.SetRPM(rpm);
        }
        */

    }

}
