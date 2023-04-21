using System;
using Aspects;
using Components;
using EnableComponents;
using Enums;
using SystemGroups;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Systems.Interactive
{
    [UpdateInGroup(typeof(InteractiveSystemGroup))]
    [UpdateAfter(typeof(DetectInteractiveObjectSystem))]
    public partial struct TakeInteractiveObjectSystem : ISystem, ISystemStartStop
    {
        private float _deltaSmoothLerp;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerComponent>();
        }

        public void OnStartRunning(ref SystemState state)
        {
            switch (SystemAPI.GetSingleton<PlayerComponent>().deltaType)
            {
                case DeltaType.Update:
                    _deltaSmoothLerp = SystemAPI.Time.DeltaTime;
                    break;
                case DeltaType.FixedUpdate:
                    _deltaSmoothLerp = SystemAPI.Time.fixedDeltaTime;
                    break;
                case DeltaType.Value:
                    break;
            }
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var handAspect in SystemAPI.Query<HandAspect>()
                         .WithDisabled<EnableDynamicState, EnableStaticState>())
            {
                if (!handAspect.IsTakeObject) continue;

                var interactiveObjectAspect = SystemAPI.GetAspectRW<InteractiveObjectAspect>(handAspect.EntityNearHand);

                if (interactiveObjectAspect.InteractiveType == InteractiveType.Dynamic)
                    SystemAPI.SetComponentEnabled<EnableDynamicState>(handAspect.GetEntity, true);
                else
                    SystemAPI.SetComponentEnabled<EnableStaticState>(handAspect.GetEntity, true);
                interactiveObjectAspect.ResetSmoothValue();
                interactiveObjectAspect.EnableInHand(handAspect.GetHandType);
            }

            foreach (var takeDynamicAspect in SystemAPI.Query<HandTakeDynamicAspect>())
            {
                var entityInteractive = takeDynamicAspect.hand.EntityInHand == Entity.Null
                    ? takeDynamicAspect.hand.EntityNearHand
                    : takeDynamicAspect.hand.EntityInHand;
                var interactiveObjectAspect =
                    SystemAPI.GetAspectRW<InteractiveObjectAspect>(entityInteractive);
                takeDynamicAspect.SmoothlyDynamicTake(interactiveObjectAspect, _deltaSmoothLerp);
            }

            foreach (var takeStaticAspect in SystemAPI.Query<HandTakeStaticAspect>())
            {
                var entityInteractive = takeStaticAspect.hand.EntityInHand == Entity.Null
                    ? takeStaticAspect.hand.EntityNearHand
                    : takeStaticAspect.hand.EntityInHand;
                var interactiveObjectAspect =
                    SystemAPI.GetAspectRW<InteractiveObjectAspect>(entityInteractive);
                takeStaticAspect.SmoothlyStaticTake(interactiveObjectAspect, _deltaSmoothLerp);
            }
        }

        public void OnStopRunning(ref SystemState state)
        {
        }
    }
}