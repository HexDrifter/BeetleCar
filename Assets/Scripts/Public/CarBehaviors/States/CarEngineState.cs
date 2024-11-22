using UnityEngine;
using Beetle.Framework;
using Beetle.Domain;

public class CarEngineState : IState
{
    protected CarBehavior Owner;

    public CarEngineState(CarBehavior owner)
    {
        Owner = owner;
    }

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void PhysicsTick() { }
    public virtual void Tick() { }


}
