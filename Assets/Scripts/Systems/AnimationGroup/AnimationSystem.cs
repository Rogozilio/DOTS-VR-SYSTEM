﻿using Aspects;
using SystemGroups;
using Unity.Burst;
using Unity.Entities;

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
            public void Execute(ref AnimationAspect animationAspect)
            {
                animationAspect.PlayHands();
            }
        }
    }
}