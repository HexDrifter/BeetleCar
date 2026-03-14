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
        var startingState = new CarStartingState(this);

        AtEngineState(engineOffState, startingState, () => inputEngineIgnition == true);
        AtEngineState(startingState, idleState, () => engineRPM >= 550f);

        AtEngineState(idleState, acceleratingState, () => inputAcceleration >= 0.1f);
        AtEngineState(acceleratingState, deceleratingState, () => inputAcceleration < 0.1f);
        AtEngineState(deceleratingState,idleState, () => engineRPM < minRPM);
        AtEngineState(deceleratingState, acceleratingState, () => inputAcceleration > 0.1f);
        AtEngineState(idleState, engineOffState, () => engineRPM < 500f);

        AtEngineState(startingState, engineOffState, () => inputEngineIgnition == false);



        _stateMachine.SetState(engineOffState);
    }

}
