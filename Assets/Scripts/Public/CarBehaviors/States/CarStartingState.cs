using UnityEngine;
using Beetle.Framework;
public class CarStartingState : CarEngineState
{
    private float _starterTorque = 5f;
    public CarStartingState(CarBehavior owner) : base(owner)
    {
    }
    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Arrancando");
        Owner.SetEngineOmega(0f, 20f);
    }
    public override void Tick()
    {
        Owner.SetEngineOmega(0f, _starterTorque);

    }
}
