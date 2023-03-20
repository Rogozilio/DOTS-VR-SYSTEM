using Unity.Entities;

namespace SystemGroups
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(AnimationSystemGroup))]
    [UpdateAfter(typeof(InputSystemGroup))]
    public class InteractiveSystemGroup : ComponentSystemGroup
    {
        
    }
}