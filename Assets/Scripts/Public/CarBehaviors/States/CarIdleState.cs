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
        //if(Owner.GetRPM()< 400)
            Owner.SetRPM(450);
    }

    public override void Tick()
    {
        base.Tick();
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
            float rpm = Mathf.Lerp(Owner.GetRPM(), Owner.minRPM + UnityEngine.Random.Range(-50f, 50f), Time.deltaTime * 2f);
            Owner.SetRPM(rpm);
        }
        

    }

}
