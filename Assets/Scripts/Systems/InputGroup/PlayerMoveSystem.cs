using Aspects;
using Components;
using EnableComponents;
using Enums;
using SystemGroups;
using Unity.Burst;
using Unity.Collections;
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
            state.Dependency = new MovePlayerJob().ScheduleParallel(state.Dependency);
            state.Dependency = new MoveHandJob()
            {
                playerPosition = SystemAPI.GetSingleton<PlayerComponent>().nextPosition,
                playerRotation = SystemAPI.GetSingleton<PlayerComponent>().nextRotation,
            }.ScheduleParallel(state.Dependency);
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
    [WithDisabled(typeof(EnableStaticState))]
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