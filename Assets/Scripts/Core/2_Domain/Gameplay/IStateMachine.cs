using UnityEngine;

namespace Beetle.Domain
{
    public interface IStateMachine
    {
        void Tick();
        void PhysicsTick();
    }
}
