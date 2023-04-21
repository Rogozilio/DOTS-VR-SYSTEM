using System;
using Components;
using EnableComponents;
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
        
        public readonly EnabledRefRW<EnableInLeftHand> inLeftHand;
        public readonly EnabledRefRW<EnableInRightHand> inRightHand;
        
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
            get
            {
                if (inLeftHand.ValueRO && inRightHand.ValueRO)
                    return InHandType.Both;
                if (inLeftHand.ValueRO && !inRightHand.ValueRO)
                    return InHandType.Left;
                if (!inLeftHand.ValueRO && inRightHand.ValueRO)
                    return InHandType.Right;
                return InHandType.None;
            }
        }

        public float ValueSmooth
        {
            get => _interactiveObject.ValueRO.valueSmooth;
            set => _interactiveObject.ValueRW.valueSmooth = value;
        }
        public HandActionType GetHandActionType
        {
            get => _interactiveObject.ValueRO.handActionType;
        }
        public InteractiveType InteractiveType
        {
            get => _interactiveObject.ValueRO.interactiveType;
            set => _interactiveObject.ValueRW.interactiveType = value;
        }

        public void ResetSmoothValue()
        {
            _interactiveObject.ValueRW.valueSmooth = _interactiveObject.ValueRO.beginValueSmooth;
        }
        public void EnableInHand(HandType handType)
        {
            switch (GetHandActionType)
            {
                case HandActionType.OneHand:
                    inLeftHand.ValueRW = handType == HandType.Left && !inRightHand.ValueRO;
                    inRightHand.ValueRW = handType == HandType.Right && !inLeftHand.ValueRO;
                    break;
                case HandActionType.FromHandToHand:
                    inLeftHand.ValueRW = handType == HandType.Left;
                    inRightHand.ValueRW = handType == HandType.Right;
                    break;
                case HandActionType.BothHand:
                    if (handType == HandType.Left)
                        inLeftHand.ValueRW = true;
                    else
                        inRightHand.ValueRW = true;
                    break;
            }
        }

        public void DisableInHand(HandType handType)
        {
            if (handType == HandType.Left)
                inLeftHand.ValueRW = false;
            else
                inRightHand.ValueRW = false;
        }
    }
}