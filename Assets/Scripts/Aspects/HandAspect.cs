using Components;
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
        public float3 GetPoseOffsetPosition => _animationAssetAspect.GetPose(GetNextPose).offsetPosition;

        private quaternion GetPoseOffsetPosition1()
        {
            var originRotate = math.mul(_animationAssetAspect.defaultPose.joints[0],
                math.inverse(_animationAssetAspect.GetPose(GetNextPose).joints[0]));
            return math.mul(input.ValueRO.rotation, originRotate);
        }

        private quaternion GetRightOffsetRotation 
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

        public float3 GetRightOffsetPosition => math.mul(GetPoseOffsetPosition1(), -GetPoseOffsetPosition);

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

        public void ClearHandData(InHandType objectInHand)
        {
            _hand.ValueRW.isReadyToTake = input.ValueRO.gripValue < 0.9f;
            var isEqualHandState = (InHandType)(_hand.ValueRO.handType + 1) == objectInHand;
            if (_hand.ValueRO.isReadyToTake || !isEqualHandState && objectInHand != InHandType.None)
            {
                _hand.ValueRW.nearHand = Entity.Null;
                _hand.ValueRW.inHand = Entity.Null;
                _hand.ValueRW.nextPose = new FixedString32Bytes();
            }
        }

        //------------Interactive Action-------------
        public void TakeObject(InteractiveObjectAspect interactiveObject, float deltaSmoothLerp)
        {
            ResetIsInHand(interactiveObject);

            if (!IsTakeObject) return;

            if (interactiveObject.InteractiveType == InteractiveType.Dynamic)
                TakeDynamicObject(interactiveObject, deltaSmoothLerp);
            else
                TakeStaticObject(interactiveObject, deltaSmoothLerp);
        }

        private void TakeDynamicObject(InteractiveObjectAspect interactiveObject, float deltaSmoothLerp)
        {
            SwitchHandState(interactiveObject);

            SmoothlyDynamicTake(interactiveObject, deltaSmoothLerp);
        }

        private void TakeStaticObject(InteractiveObjectAspect interactiveObject, float deltaSmoothLerp)
        {
            SwitchHandState(interactiveObject);

            SmoothlyStaticTake(interactiveObject, deltaSmoothLerp);
        }

        private void ResetIsInHand(InteractiveObjectAspect interactiveObject)
        {
            if (interactiveObject.InHand == (InHandType)_hand.ValueRO.handType + 1 && !IsTakeObject)
                interactiveObject.InHand = InHandType.None;
        }

        private void SwitchHandState(InteractiveObjectAspect interactiveObject)
        {
            if (interactiveObject.InHand == InHandType.None)
            {
                _hand.ValueRW.inHand = interactiveObject.GetEntity;
                interactiveObject.InHand = (InHandType)(_hand.ValueRO.handType + 1);
                interactiveObject.ValueSmooth = interactiveObject.GetBeginValueSmooth;
            }

            if (_hand.ValueRO.inHand != Entity.Null) return;
            if (interactiveObject.InHand == InHandType.Left && _hand.ValueRO.handType == HandType.Right)
            {
                _hand.ValueRW.inHand = interactiveObject.GetEntity;
                interactiveObject.InHand = InHandType.Right;
                interactiveObject.ValueSmooth = interactiveObject.GetBeginValueSmooth;
            }
            else if (interactiveObject.InHand == InHandType.Right && _hand.ValueRO.handType == HandType.Left)
            {
                _hand.ValueRW.inHand = interactiveObject.GetEntity;
                interactiveObject.InHand = (InHandType)(_hand.ValueRO.handType + 1);
                interactiveObject.ValueSmooth = interactiveObject.GetBeginValueSmooth;
            }
        }

        private void SmoothlyDynamicTake(InteractiveObjectAspect interactiveObject, float delta)
        {
            var startPos = interactiveObject.worldTransform.ValueRO.Position;
            var finishPos = localTransform.ValueRO.Position + GetRightOffsetPosition;
            var lerpPosition = math.lerp(startPos, finishPos, interactiveObject.ValueSmooth);
            interactiveObject.localTransform.ValueRW.Position = lerpPosition;

            var startRot = interactiveObject.localTransform.ValueRO.Rotation;
            var finishRot = math.mul(localTransform.ValueRO.Rotation, GetRightOffsetRotation);
            var lerpRotation = math.nlerp(startRot, finishRot, interactiveObject.ValueSmooth);
            interactiveObject.localTransform.ValueRW.Rotation = lerpRotation;

            interactiveObject.ValueSmooth =
                math.clamp(interactiveObject.ValueSmooth + delta, 0f, 1f);
        }

        private void SmoothlyStaticTake(InteractiveObjectAspect interactiveObject, float delta)
        {
            var startPos = localTransform.ValueRO.Position + GetRightOffsetPosition;
            var finishPos = interactiveObject.worldTransform.ValueRO.Position;
            var lerpPosition = math.lerp(startPos, finishPos, interactiveObject.ValueSmooth);
            localTransform.ValueRW.Position = lerpPosition;

            var startRot = localTransform.ValueRO.Rotation;
            var finishRot = interactiveObject.localTransform.ValueRO.Rotation;
            var lerpRotation = math.nlerp(startRot, finishRot, interactiveObject.ValueSmooth);
            localTransform.ValueRW.Rotation = lerpRotation;

            interactiveObject.ValueSmooth =
                math.clamp(interactiveObject.ValueSmooth + delta, 0f, 1f);
        }
    }
}