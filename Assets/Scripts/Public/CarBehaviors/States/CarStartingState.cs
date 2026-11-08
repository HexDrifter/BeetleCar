using UnityEngine;
using Beetle.Framework;
public class CarStartingState : CarEngineState
{
    private readonly float _starterTorque = 35f;
    public CarStartingState(CarBehavior owner) : base(owner)
    {
    }
    public override void OnEnter()
    {
        base.OnEnter();
        Debug.Log("Arrancando");
        // OnEnter no integra fisica: la simulacion ocurre una sola vez por paso fijo.
    }
    public override void Tick()
    {
        Owner.SimulatePowertrain(0f, _starterTorque);


    }
}
