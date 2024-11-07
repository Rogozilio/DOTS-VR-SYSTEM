using Aspects;
using SystemGroups;
using Tags;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    [UpdateAfter(typeof(MoveHandSystem))]
    public partial struct MovePhysicsHandSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LeftHandTag>();
            state.RequireForUpdate<RightHandTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var leftHand = SystemAPI.GetSingletonEntity<LeftHandTag>();
            var rightHand = SystemAPI.GetSingletonEntity<RightHandTag>();

            state.Dependency = new MoveLeftPhysicsHand()
            {
                fixedDeltaTime = SystemAPI.Time.fixedDeltaTime,
                positionHand = SystemAPI.GetComponentRO<LocalTransform>(leftHand).ValueRO.Position,
                rotationHand = SystemAPI.GetComponentRO<LocalTransform>(leftHand).ValueRO.Rotation
            }.Schedule(state.Dependency);

            state.Dependency = new MoveRightPhysicsHand()
            {
                fixedDeltaTime = SystemAPI.Time.fixedDeltaTime,
                positionHand = SystemAPI.GetComponentRO<LocalTransform>(rightHand).ValueRO.Position,
                rotationHand = SystemAPI.GetComponentRO<LocalTransform>(rightHand).ValueRO.Rotation
            }.Schedule(state.Dependency);
        }

        [BurstCompile]
        public partial struct MoveLeftPhysicsHand : IJobEntity
        {
            public float fixedDeltaTime;
            public float3 positionHand;
            public quaternion rotationHand;

            private void Execute(PhysicsHandAspect aspect, ref PhysicsVelocity physicsVelocity,
                in PhysicsLeftHandTag tag)
            {
                physicsVelocity.Linear = aspect.CalculateVelocityLinear(positionHand, fixedDeltaTime);
                physicsVelocity.Angular = aspect.CalculateVelocityAngle(rotationHand, fixedDeltaTime);
            }
        }

        [BurstCompile]
        public partial struct MoveRightPhysicsHand : IJobEntity
        {
            public float fixedDeltaTime;
            public float3 positionHand;
            public quaternion rotationHand;

            private void Execute(PhysicsHandAspect aspect, ref PhysicsVelocity physicsVelocity,
                in PhysicsRightHandTag tag)
            {
                physicsVelocity.Linear = aspect.CalculateVelocityLinear(positionHand, fixedDeltaTime);
                physicsVelocity.Angular = aspect.CalculateVelocityAngle(rotationHand, fixedDeltaTime);
            }
        }
    }
}