using Components;
using EnableComponents;
using Enums;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Aspects
{
    public readonly partial struct HandAspect : IAspect
    {
        private readonly Entity _entity;
        private readonly RefRW<Hand> _hand;
        private readonly AnimationAssetAspect _animationAssetAspect;

        public readonly RefRO<InputHand> input;
        public readonly RefRW<LocalTransform> localTransform;
        public Entity GetEntity => _entity;

        public bool IsReadyToTake
        {
            get => _hand.ValueRO.isReadyToTake;
            set => _hand.ValueRW.isReadyToTake = value;
        }
        public quaternion GetDefaultPoseRotation => _animationAssetAspect.defaultPose.joints[0];

        public bool IsTakeObject => EntityNearHand != Entity.Null && input.ValueRO.gripValue > 0.9f;
        public HandType GetHandType => _hand.ValueRO.handType;

        public Entity EntityNearHand
        {
            get => _hand.ValueRO.nearHand;
            set => _hand.ValueRW.nearHand = value;
        }

        public Entity EntityInHand
        {
            get => _hand.ValueRO.inHand;
            set => _hand.ValueRW.inHand = value;
        }

        public string GetNextPose => _hand.ValueRO.nextPose.ToString();
        public float3 GetOffsetPositionPose => _animationAssetAspect.GetPose(GetNextPose).offsetPosition;

        private quaternion GetRotatePose()
        {
            var deltaRotateDefaultAndNextPose = math.mul(_animationAssetAspect.defaultPose.joints[0],
                math.inverse(_animationAssetAspect.GetPose(GetNextPose).joints[0]));
            return math.mul(localTransform.ValueRO.Rotation, deltaRotateDefaultAndNextPose);
        }

        public quaternion GetRightOffsetRotation
        {
            get
            {
                if (GetNextPose == "")
                    return quaternion.identity;
                return math.mul(_animationAssetAspect.defaultPose.joints[0],
                    math.inverse(_animationAssetAspect.GetPose(GetNextPose).joints[0]));
            }
        }

        public float3 GetRightPosition => input.ValueRO.position + _hand.ValueRO.offsetPosition;

        public float3 GetRightOffsetPosition => math.mul(GetRotatePose(), -GetOffsetPositionPose);

        public quaternion GetRightRotation => math.dot(_hand.ValueRO.offsetRotation, quaternion.identity) != 0
            ? math.mul(input.ValueRO.rotation, _hand.ValueRO.offsetRotation)
            : input.ValueRO.rotation;

        public int GetLenghtJoints => _hand.ValueRO.joints.Length;

        public void SetJoint(int index, quaternion value)
        {
            _hand.ValueRW.joints[index] = value;
        }

        public quaternion GetJoint(int index)
        {
            return _hand.ValueRO.joints[index];
        }

        public void ClearHandData()
        {
            _hand.ValueRW.nearHand = Entity.Null;
            //_hand.ValueRW.inHand = Entity.Null;
            _hand.ValueRW.nextPose = new FixedString32Bytes();
        }
    }
}