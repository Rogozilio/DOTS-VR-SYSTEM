using Aspects;
using Components;
using Enums;
using SystemGroups;
using TriggerEventJob;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;

namespace Systems.Interactive
{
    [UpdateInGroup(typeof(InteractiveSystemGroup), OrderFirst = true)]
    public partial struct DetectInteractiveObjectSystem : ISystem
    {
        private ComponentLookup<Hand> handLookup;
        private ComponentLookup<InteractiveObject> interactiveLookup;
        private ComponentLookup<LocalToWorld> worldTransformLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            handLookup = SystemAPI.GetComponentLookup<Hand>();
            interactiveLookup = SystemAPI.GetComponentLookup<InteractiveObject>();
            worldTransformLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();

            handLookup.Update(ref state);
            interactiveLookup.Update(ref state);
            worldTransformLookup.Update(ref state);

            state.Dependency = new ClearInteractiveObjectsJob().ScheduleParallel(state.Dependency);
            state.Dependency = new ClearHandDataJob()
            {
                interactiveObjects = interactiveLookup
            }.ScheduleParallel(state.Dependency);
            state.Dependency = new TriggerHands()
            {
                hands = handLookup,
                interactiveObjects = interactiveLookup,
                worldTransform = worldTransformLookup
            }.Schedule(simulation, state.Dependency);
        }

        [BurstCompile]
        private partial struct ClearHandDataJob : IJobEntity
        {
            [ReadOnly] public ComponentLookup<InteractiveObject> interactiveObjects;

            public void Execute(ref HandAspect handAspect)
            {
                handAspect.ClearHandData(handAspect.EntityInHand != Entity.Null
                    ? interactiveObjects[handAspect.EntityInHand].inHand
                    : InHandType.None);
            }
        }

        [BurstCompile]
        private partial struct ClearInteractiveObjectsJob : IJobEntity
        {
            public void Execute(ref InteractiveObject interactiveObjectAspect)
            {
                interactiveObjectAspect.distanceToHand = 0;
            }
        }
    }
}