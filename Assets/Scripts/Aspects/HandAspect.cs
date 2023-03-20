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
        private readonly Entity entity;
        private readonly RefRW<Hand> hand;
        private readonly AnimationAssetAspect animationAssetAspect;

        public readonly RefRO<InputHand> input;
        public readonly TransformAspect transform;

        public Entity GetEntity => entity;

        public bool IsTakeObject => EntityInHand != Entity.Null && input.ValueRO.gripValue > 0.9f;

        public Entity EntityInHand
        {
            get => hand.ValueRO.inHand;
            set => hand.ValueRW.inHand = value;
        }

        public string GetNextPose => hand.ValueRO.nextPose.ToString();
        public float3 GetPoseOffsetPosition => animationAssetAspect.GetPose(GetNextPose).offsetPosition;
        public float3 GetRightPosition => input.ValueRO.position + hand.ValueRO.offsetPosition;

        public float3 SetOffsetPosition
        {
            set => hand.ValueRW.offsetPosition = value;
        }
        
        public float3 GetRightOffsetPose =>  math.mul(transform.LocalRotation, -GetPoseOffsetPosition);

        public quaternion GetRightRotation => math.mul(input.ValueRO.rotation, hand.ValueRO.offsetRotation);

        public quaternion SetOffsetRotation
        {
            set => hand.ValueRW.offsetRotation = value;
        }

        public int GetLenghtJoints => hand.ValueRO.joints.Length;

        public void SetJoint(int index, quaternion value)
        {
            hand.ValueRW.joints[index] = value;
        }

        public quaternion GetJoint(int index)
        {
            return hand.ValueRO.joints[index];
        }

        public void ClearHandData()
        {
            hand.ValueRW.isReadyToTake = input.ValueRO.gripValue < 0.9f;
            hand.ValueRW.inHand = hand.ValueRO.isReadyToTake ? Entity.Null : hand.ValueRO.inHand;
            hand.ValueRW.nextPose = hand.ValueRO.isReadyToTake ? new FixedString32Bytes() : hand.ValueRO.nextPose;
        }

        //------------Interactive Action-------------

        public void TakeObject(InteractiveObjectAspect interactiveObject)
        {
            if (!IsTakeObject) return;

            if (interactiveObject.SmoothlyState == SmoothlyState.End)
            {
                interactiveObject.transform.WorldPosition =
                    transform.WorldPosition + GetRightOffsetPose;
                interactiveObject.transform.LocalRotation = transform.LocalRotation;
            }
            else
            {
                SmoothlyTake(interactiveObject);
            }

            SetOffsetRotationByName(GetNextPose);
        }

        private void SmoothlyTake(InteractiveObjectAspect interactiveObject)
        {
            interactiveObject.SmoothlyState = SmoothlyState.Progress;
            var start = interactiveObject.transform.WorldPosition;
            var finish = transform.WorldPosition + GetRightOffsetPose;
            interactiveObject.transform.WorldPosition = math.lerp(start, finish, 0.2f);
            interactiveObject.transform.LocalRotation =
                math.nlerp(interactiveObject.transform.LocalRotation, transform.LocalRotation, 0.2f);

            StopSmoothly(interactiveObject);
        }

        private void StopSmoothly(InteractiveObjectAspect interactiveObject)
        {
            if (math.distance(interactiveObject.transform.WorldPosition,
                    transform.WorldPosition + GetRightOffsetPose) < 0.01f)
            {
                interactiveObject.SmoothlyState = SmoothlyState.End;
            }
        }

        private void SetOffsetRotationByName(string namePose)
        {
            SetOffsetRotation = math.mul(animationAssetAspect.defaultPose.joints[0],
                math.inverse(animationAssetAspect.GetPose(namePose).joints[0]));
        }
    }
}