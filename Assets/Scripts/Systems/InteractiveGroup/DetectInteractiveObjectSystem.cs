using Aspects;
using Components;
using SystemGroups;
using TriggerEventJob;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Systems.Interactive
{
    [UpdateInGroup(typeof(InteractiveSystemGroup), OrderFirst = true)]
    public partial struct DetectInteractiveObjectSystem : ISystem
    {
        private ComponentLookup<Hand> handLookup;
        private ComponentLookup<InteractiveObject> interactiveLookup;
        private ComponentLookup<WorldTransform> worldTransformLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            handLookup = SystemAPI.GetComponentLookup<Hand>();
            interactiveLookup = SystemAPI.GetComponentLookup<InteractiveObject>();
            worldTransformLookup = SystemAPI.GetComponentLookup<WorldTransform>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            
            handLookup.Update(ref state);
            interactiveLookup.Update(ref state);
            worldTransformLookup.Update(ref state);

            var clearInteractiveJob = new ClearDistanceInteractiveObjectsJob().ScheduleParallel(state.Dependency);
            var clearHandJob = new ClearHandDataJob().ScheduleParallel(state.Dependency);

            state.Dependency = JobHandle.CombineDependencies(clearInteractiveJob, clearHandJob);
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
            public void Execute(ref HandAspect handAspect)
            {
                handAspect.ClearHandData();
            }
        }
        [BurstCompile]
        private partial struct ClearDistanceInteractiveObjectsJob : IJobEntity
        {
            public void Execute(ref InteractiveObject interactiveObjectAspect)
            {
                interactiveObjectAspect.distanceToHand = 0;
            }
        }
    }
}