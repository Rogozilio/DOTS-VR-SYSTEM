using Components;
using Enums;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Aspects
{
    public readonly partial struct InteractiveObjectAspect : IAspect
    {
        private readonly Entity _entity;
        private readonly RefRW<InteractiveObject> _interactiveObject;
        
        public readonly RefRW<LocalTransform> localTransform; 
        public readonly RefRO<LocalToWorld> worldTransform;

        public Entity GetEntity => _entity;
        public float DistanceToHand
        {
            get => _interactiveObject.ValueRO.distanceToHand;
            set => _interactiveObject.ValueRW.distanceToHand = value;
        }

        public InHandType InHand
        {
            get => _interactiveObject.ValueRO.inHand;
            set => _interactiveObject.ValueRW.inHand = value;
        }

        public float ValueSmooth
        {
            get => _interactiveObject.ValueRO.valueSmooth;
            set => _interactiveObject.ValueRW.valueSmooth = value;
        }

        public InteractiveType InteractiveType
        {
            get => _interactiveObject.ValueRO.interactiveType;
            set => _interactiveObject.ValueRW.interactiveType = value;
        }

        public float GetBeginValueSmooth => _interactiveObject.ValueRO.beginValueSmooth;
    }
}