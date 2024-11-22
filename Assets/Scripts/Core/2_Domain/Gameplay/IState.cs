using UnityEngine;

namespace Beetle.Domain
{
    public interface IState
    {
        void Tick();
        void PhysicsTick();
        void OnEnter();
        void OnExit();
    }
}
