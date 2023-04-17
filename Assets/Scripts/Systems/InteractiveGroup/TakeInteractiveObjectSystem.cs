using System;
using Aspects;
using Components;
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
    public partial struct TakeInteractiveObjectSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var handAspect in SystemAPI.Query<HandAspect>())
            {
                if (handAspect.EntityNearHand == Entity.Null) continue;

                float deltaSmoothLerp = 0;
                switch (SystemAPI.GetSingleton<PlayerComponent>().deltaType)
                {
                    case DeltaType.Update:
                        deltaSmoothLerp = SystemAPI.Time.DeltaTime;
                        break;
                    case DeltaType.FixedUpdate:
                        deltaSmoothLerp = SystemAPI.Time.fixedDeltaTime;
                        break;
                    case DeltaType.Value:
                        break;
                }

                var interactiveObjectAspect = SystemAPI.GetAspectRW<InteractiveObjectAspect>(handAspect.EntityNearHand);
                handAspect.TakeObject(interactiveObjectAspect, deltaSmoothLerp);
            }
        }
    }
}