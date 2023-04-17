using Aspects;
using Components;
using SystemGroups;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    [UpdateAfter(typeof(InputSystem))]
    public partial struct PlayerMoveSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var playerJob = new MovePlayerJob();
            playerJob.ScheduleParallel();
            
            var handJob = new MoveHandJob()
            {
                playerPosition = SystemAPI.GetSingleton<PlayerComponent>().nextPosition,
                playerRotation = SystemAPI.GetSingleton<PlayerComponent>().nextRotation
            };
            handJob.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct MovePlayerJob : IJobEntity
    {
        public void Execute(ref LocalTransform localTransform, in PlayerComponent player)
        {
            localTransform.Position = player.nextPosition;
            localTransform.Rotation = player.nextRotation;
        }
    }

    [BurstCompile]
    public partial struct MoveHandJob : IJobEntity
    {
        public float3 playerPosition;
        public quaternion playerRotation;
        public void Execute(ref HandAspect handAspect)
        {
            handAspect.localTransform.ValueRW.Position = playerPosition + math.mul(playerRotation, handAspect.GetRightPosition);
            handAspect.localTransform.ValueRW.Rotation =  math.mul(playerRotation, handAspect.GetRightRotation);
        }
    }
}