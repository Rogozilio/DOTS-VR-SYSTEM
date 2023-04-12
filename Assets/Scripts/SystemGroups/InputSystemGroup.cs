using Unity.Entities;
using Unity.Transforms;

namespace SystemGroups
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(TransformSystemGroup))]
    public partial class InputSystemGroup : ComponentSystemGroup
    {
        
    }
}