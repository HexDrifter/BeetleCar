using UnityEngine;
using Beetle.Framework;

public class VWBeetleCarBehavior : CarBehavior
{
    public override void Initialize()
    {
        base.Initialize();

        var idleState = new CarIdleState(this);
    }

}
