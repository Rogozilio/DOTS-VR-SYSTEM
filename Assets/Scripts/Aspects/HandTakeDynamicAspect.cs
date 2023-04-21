using EnableComponents;
using Enums;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Aspects
{
    public readonly partial struct HandTakeDynamicAspect : IAspect
    {
        public readonly HandAspect hand;
        private readonly EnabledRefRW<EnableDynamicState> _enableDynamicState;

        public void SmoothlyDynamicTake(InteractiveObjectAspect interactiveObject, float delta)
        {
            if(hand.EntityInHand == Entity.Null)
                hand.EntityInHand = interactiveObject.GetEntity;

            var startPos = interactiveObject.worldTransform.ValueRO.Position;
            var finishPos = hand.localTransform.ValueRO.Position + hand.GetRightOffsetPosition;
            var lerpPosition = math.lerp(startPos, finishPos, interactiveObject.ValueSmooth);
            interactiveObject.localTransform.ValueRW.Position = lerpPosition;

            var startRot = interactiveObject.localTransform.ValueRO.Rotation;
            var finishRot = math.mul(hand.localTransform.ValueRO.Rotation, hand.GetRightOffsetRotation);
            var lerpRotation = math.nlerp(startRot, finishRot, interactiveObject.ValueSmooth);
            interactiveObject.localTransform.ValueRW.Rotation = lerpRotation;

            interactiveObject.ValueSmooth =
                math.clamp(interactiveObject.ValueSmooth + delta, 0f, 1f);

            if(!hand.IsReadyToTake && (InHandType)(hand.GetHandType + 1) == interactiveObject.InHand) return;
            
            ResetData(interactiveObject);
        }

        private void ResetData(InteractiveObjectAspect interactiveObject)
        {
            hand.ClearHandData();
            hand.EntityInHand = Entity.Null;
            _enableDynamicState.ValueRW = false;
            interactiveObject.DisableInHand(hand.GetHandType);
        }
    }
}