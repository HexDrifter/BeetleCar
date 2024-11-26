using UnityEngine;
using Beetle.Framework;

public class VWBeetleCarBehavior : CarBehavior
{
    public override void Initialize()
    {
        base.Initialize();

        var idleState = new CarIdleState(this);
        var acceleratingState = new CarAcceleratingState(this);
        var deceleratingState = new CarDeceleratingState(this);
        var engineOffState = new CarEngineOffState(this);

        AtEngineState(idleState, acceleratingState, () => inputAcceleration >= 0.1f);
        AtEngineState(acceleratingState, deceleratingState, () => inputAcceleration < 0.1f);
        AtEngineState(deceleratingState,idleState, () => engineRPM < minRPM);
        AtEngineState(deceleratingState, acceleratingState, () => inputAcceleration > 0.1f);
        AtEngineState(idleState, engineOffState, () => engineRPM < 350f);

        AtEngineState(engineOffState, idleState, () => inputEngineIgnition == true);

        _stateMachine.SetState(engineOffState);
    }

}
