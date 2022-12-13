using Components;
using SystemGroups;
using Tags;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(InputSystemGroup))]
    [UpdateAfter(typeof(InputSystem))]
    public partial struct HandMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
           
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var job = new MoveHandJob();
            job.ScheduleParallel();
        }
    }
    
    [BurstCompile]
    public partial struct MoveHandJob : IJobEntity
    {
        public void Execute(ref TransformAspect transform, in InputHand inputHand)
        {
            transform.LocalPosition = inputHand.position;
            transform.LocalRotation = inputHand.rotation;
        }
    }
}