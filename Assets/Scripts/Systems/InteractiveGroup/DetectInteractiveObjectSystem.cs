using Aspects;
using Components;
using EnableComponents;
using SystemGroups;
using TriggerEventJob;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Systems.Interactive
{
    [UpdateInGroup(typeof(InteractiveSystemGroup), OrderFirst = true)]
    public partial struct DetectInteractiveObjectSystem : ISystem
    {
        private ComponentLookup<Hand> _handLookup;
        private ComponentLookup<EnableInLeftHand> _inLeftHandLookup;
        private ComponentLookup<EnableInRightHand> _inRightHandLookup;
        private ComponentLookup<InteractiveObject> _interactiveLookup;
        private ComponentLookup<LocalToWorld> _worldTransformLookup;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _handLookup = SystemAPI.GetComponentLookup<Hand>();
            _inLeftHandLookup = SystemAPI.GetComponentLookup<EnableInLeftHand>();
            _inRightHandLookup = SystemAPI.GetComponentLookup<EnableInRightHand>();
            _interactiveLookup = SystemAPI.GetComponentLookup<InteractiveObject>();
            _worldTransformLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();

            _handLookup.Update(ref state);
            _inLeftHandLookup.Update(ref state);
            _inRightHandLookup.Update(ref state);
            _interactiveLookup.Update(ref state);
            _worldTransformLookup.Update(ref state);
            
            var jobClearInteractiveObjectData = new ClearInteractiveObjectsJob().ScheduleParallel(state.Dependency);
            var jobClearHandData = new ClearHandDataJob().ScheduleParallel(state.Dependency);
            state.Dependency = JobHandle.CombineDependencies(jobClearInteractiveObjectData, jobClearHandData);
            state.Dependency = new TriggerHands()
            {
                hands = _handLookup,
                interactiveObjects = _interactiveLookup,
                inLeftHand = _inLeftHandLookup,
                inRightHand = _inRightHandLookup,
                worldTransform = _worldTransformLookup
            }.Schedule(simulation, state.Dependency);
        }

        [BurstCompile]
        private partial struct ClearHandDataJob : IJobEntity
        {
            public void Execute(ref HandAspect handAspect)
            {
                handAspect.IsReadyToTake = handAspect.input.ValueRO.gripValue < 0.9f;
                if(handAspect.IsReadyToTake) handAspect.ClearHandData();
            }
        }

        [BurstCompile]
        private partial struct ClearInteractiveObjectsJob : IJobEntity
        {
            public void Execute(ref InteractiveObject interactiveObject)
            {
                interactiveObject.distanceToHand = 0;
            }
        }
    }
}