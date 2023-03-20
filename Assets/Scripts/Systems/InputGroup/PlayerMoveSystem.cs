using Aspects;
using Components;
using SystemGroups;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    [UpdateAfter(typeof(InputSystem))]
    public partial struct PlayerMoveSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var playerJob = new MovePlayerJob();
            playerJob.ScheduleParallel();
            
            var handJob = new MoveHandJob()
            {
                playerPosition = SystemAPI.GetSingleton<PlayerComponent>().nextPosition
            };
            handJob.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct MovePlayerJob : IJobEntity
    {
        public void Execute(ref TransformAspect transform, in PlayerComponent player)
        {
            transform.LocalPosition = player.nextPosition;
            transform.LocalRotation = player.nextRotation;
        }
    }

    [BurstCompile]
    public partial struct MoveHandJob : IJobEntity
    {
        public float3 playerPosition;
        public void Execute(ref HandAspect handAspect)
        {
            handAspect.transform.WorldPosition = playerPosition + handAspect.GetRightPosition;
            handAspect.transform.LocalRotation = handAspect.GetRightRotation;
        }
    }
}