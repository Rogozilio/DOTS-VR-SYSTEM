using EnableComponents;
using Enums;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Aspects
{
    public readonly partial struct HandTakeStaticAspect : IAspect
    {
        public readonly HandAspect hand;
        private readonly EnabledRefRW<EnableStaticState> _enableStaticState;

        public void SmoothlyStaticTake(InteractiveObjectAspect interactiveObject, float delta)
        {
            if(hand.EntityInHand == Entity.Null)
                hand.EntityInHand = interactiveObject.GetEntity;

            var startPos = hand.localTransform.ValueRO.Position;
            var finishPos = interactiveObject.worldTransform.ValueRO.Position + hand.GetOffsetPositionPose;
            var lerpPosition = math.lerp(startPos, finishPos, interactiveObject.ValueSmooth);
            hand.localTransform.ValueRW.Position = lerpPosition;

            var startRot = hand.localTransform.ValueRO.Rotation;
            var finishRot = interactiveObject.localTransform.ValueRO.Rotation;
            var lerpRotation = math.nlerp(startRot, finishRot, interactiveObject.ValueSmooth);
            hand.localTransform.ValueRW.Rotation = lerpRotation;

            interactiveObject.ValueSmooth =
                math.clamp(interactiveObject.ValueSmooth + delta, 0f, 1f);
            
            if(!hand.IsReadyToTake && (InHandType)(hand.GetHandType + 1) == interactiveObject.InHand) return;
            
            ResetData(interactiveObject);
        }
        
        private void ResetData(InteractiveObjectAspect interactiveObject)
        {
            hand.ClearHandData();
            hand.EntityInHand = Entity.Null;
            _enableStaticState.ValueRW = false;
            interactiveObject.DisableInHand(hand.GetHandType);
        }
    }
}