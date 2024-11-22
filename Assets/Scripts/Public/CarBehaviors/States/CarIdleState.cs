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
        //Owner;
    }
}
