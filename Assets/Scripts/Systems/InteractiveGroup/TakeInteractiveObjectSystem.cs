using Aspects;
using Enums;
using SystemGroups;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace Systems.Interactive
{
    [UpdateInGroup(typeof(InteractiveSystemGroup))]
    [UpdateAfter(typeof(DetectInteractiveObjectSystem))]
    public partial struct TakeInteractiveObjectSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach(var handAspect in SystemAPI.Query<HandAspect>())
            {
                if(handAspect.EntityInHand == Entity.Null) continue;
                
                var interactiveObjectAspect = SystemAPI.GetAspectRW<InteractiveObjectAspect>(handAspect.EntityInHand);
                handAspect.TakeObject(interactiveObjectAspect);
            }
        }
    }
}