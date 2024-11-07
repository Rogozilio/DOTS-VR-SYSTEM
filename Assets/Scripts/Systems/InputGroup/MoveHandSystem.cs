using Components;
using SystemGroups;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    [UpdateInGroup(typeof(InputSystemGroup))]
    public partial struct MoveHandSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new MoveJob().Schedule(state.Dependency);
        }
        
        [BurstCompile]
        private partial struct MoveJob : IJobEntity
        {
            private void Execute(ref LocalTransform localTransform, in InputHand inputHand)
            {
                localTransform.Position = inputHand.position;
                localTransform.Rotation = inputHand.rotation;
            }
        }
    }
}