using System;
using Aspects;
using SystemGroups;
using Unity.Burst;
using Unity.Entities;
using Object = UnityEngine.Object;

namespace Systems
{
    [UpdateInGroup(typeof(AnimationSystemGroup))]
    [UpdateAfter(typeof(AnimationReturnInComponent))]
    public partial struct AnimationSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var moveHandJob = new MoveHandJob();
            moveHandJob.ScheduleParallel();
        }
        
        [BurstCompile]
        public partial struct MoveHandJob : IJobEntity
        {
            public void Execute(AnimationAspect animationAspect)
            {
                animationAspect.PlayHands();
            }
        }
    }
}