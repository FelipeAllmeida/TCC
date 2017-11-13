using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework;

public class GatherCommand : Command
{
    private Entity _actor;
    private Resource _resource;
    private Action _actionFinishGathering;
    private TimerNodule _gatherNodule;

    public GatherCommand(Entity p_actor, Resource p_resource, Action p_callbackFinish = null)
    {
        _actor = p_actor;
        _resource = p_resource;
        _actionFinishGathering = p_callbackFinish;
    }

    public override CommandType Execute()
    {
        GatherRecursion();
        return CommandType.GATHER;
    }

    private void GatherRecursion()
    {
        if (_resource.isDepleted == true)
        {
            Stop();
            return;
        }
        _gatherNodule = Timer.WaitSeconds(_resource.GetExtractionTime(), delegate
        {
            int __resourcesToAdd = _resource.GatherResource(_actor.GetEntityGatherEfficiency());
            _actor.AddResource(_resource.GetResourceType(), __resourcesToAdd);
            GatherRecursion();
        });
    }

    public override void Stop()
    {
        if (_gatherNodule != null) _gatherNodule.Stop();
        _actionFinishGathering = null;
    }
}
