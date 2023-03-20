using Unity.Entities;
using Unity.Transforms;

namespace SystemGroups
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    [UpdateAfter(typeof(InputSystemGroup))]
    public class AnimationSystemGroup : ComponentSystemGroup
    {
        
    }
}