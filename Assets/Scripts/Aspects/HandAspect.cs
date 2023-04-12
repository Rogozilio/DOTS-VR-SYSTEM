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
        public float3 GetRightPosition => input.ValueRO.position + _hand.ValueRO.offsetPosition;

        public float3 SetOffsetPosition
        {
            set => _hand.ValueRW.offsetPosition = value;
        }

        public float3 GetRightOffsetPose => math.mul(localTransform.ValueRO.Rotation, -GetPoseOffsetPosition);

        public quaternion GetRightRotation => math.mul(input.ValueRO.rotation, _hand.ValueRO.offsetRotation);

        public quaternion SetOffsetRotation
        {
            set => _hand.ValueRW.offsetRotation = value;
        }

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
            
            SwitchHandState(interactiveObject);
            
            SmoothlyTake(interactiveObject, deltaSmoothLerp);
            
            SetOffsetRotationByName(GetNextPose);
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
            if(_hand.ValueRO.inHand != Entity.Null) return;
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

        private void SmoothlyTake(InteractiveObjectAspect interactiveObject, float delta)
        {
            var startPos = interactiveObject.worldTransform.ValueRO.Position;
            var finishPos = localTransform.ValueRO.Position + GetRightOffsetPose;
            var lerpPosition = math.lerp(startPos, finishPos, interactiveObject.ValueSmooth);
            interactiveObject.localTransform.ValueRW.Position = lerpPosition;
            
            var startRot = interactiveObject.localTransform.ValueRO.Rotation;
            var finishRot = localTransform.ValueRO.Rotation;
            var lerpRotation = math.nlerp(startRot, finishRot, interactiveObject.ValueSmooth);
            interactiveObject.localTransform.ValueRW.Rotation = lerpRotation;
            
            interactiveObject.ValueSmooth =
                math.clamp(interactiveObject.ValueSmooth + delta, 0f, 1f);
        }

        private void SetOffsetRotationByName(string namePose)
        {
            SetOffsetRotation = math.mul(_animationAssetAspect.defaultPose.joints[0],
                math.inverse(_animationAssetAspect.GetPose(namePose).joints[0]));
        }
    }
}