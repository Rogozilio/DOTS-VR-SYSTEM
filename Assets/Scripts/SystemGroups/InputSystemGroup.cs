using Unity.Entities;
using Unity.Transforms;

namespace SystemGroups
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class InputSystemGroup : ComponentSystemGroup
    {
        
    }
}