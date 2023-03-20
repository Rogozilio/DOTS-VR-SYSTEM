using System;
using Components;
using Enums;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Aspects
{
    public readonly partial struct InteractiveObjectAspect : IAspect
    {
        public readonly TransformAspect transform;
        private readonly RefRW<InteractiveObject> interactiveObject;

        public float DistanceToHand
        {
            get => interactiveObject.ValueRO.distanceToHand;
            set => interactiveObject.ValueRW.distanceToHand = value;
        }

        public SmoothlyState SmoothlyState
        {
            get => interactiveObject.ValueRO.smoothlyState;
            set
            {
                if (value == SmoothlyState.Start)
                {
                    if(interactiveObject.ValueRO.smoothlyState == SmoothlyState.End)
                        interactiveObject.ValueRW.smoothlyState = value;
                    return;
                }
                interactiveObject.ValueRW.smoothlyState = value;
            }
        }
    }
}