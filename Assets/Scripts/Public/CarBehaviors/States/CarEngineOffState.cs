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
        Owner.SimulatePowertrain(0f, 0f, combustionEnabled: false);

    }

    public override void OnExit()
    {
        base.OnExit();
        //Owner.SetRPM(450);
    }
}
