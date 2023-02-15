using Components;
using SystemGroups;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [BurstCompile]
    [UpdateInGroup(typeof(InputSystemGroup))]
    [UpdateAfter(typeof(InputSystem))]
    public partial struct PlayerMoveSystem : ISystem
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
            var playerJob = new MovePlayerJob();
            var handJob = new MoveHandJob();

            playerJob.ScheduleParallel();
            handJob.ScheduleParallel();
        }
    }

    [BurstCompile]
    public partial struct MovePlayerJob : IJobEntity
    {
        public void Execute(ref TransformAspect transform, ref PlayerComponent player)
        {
            transform.LocalPosition = player.nextPosition;
        }
    }

    [BurstCompile]
    public partial struct MoveHandJob : IJobEntity
    {
        public void Execute(ref TransformAspect transform, in InputHand inputHand)
        {
            transform.LocalPosition = inputHand.position + inputHand.offsetPosition;
            transform.LocalRotation = math.mul(inputHand.rotation, inputHand.offsetRotation);
        }
    }
}