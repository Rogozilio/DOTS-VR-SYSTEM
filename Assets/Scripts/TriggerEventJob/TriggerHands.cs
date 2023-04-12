using Components;
using Enums;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace TriggerEventJob
{
    [BurstCompile]
    public struct TriggerHands : ITriggerEventsJob
    {
        public ComponentLookup<Hand> hands;
        public ComponentLookup<InteractiveObject> interactiveObjects;

        [ReadOnly] public ComponentLookup<LocalToWorld> worldTransform;

        public void Execute(TriggerEvent triggerEvent)
        {
            Entity handEntity = Entity.Null;
            Entity interactiveEntity = Entity.Null;

            if (hands.HasComponent(triggerEvent.EntityA))
                handEntity = triggerEvent.EntityA;
            else if (hands.HasComponent(triggerEvent.EntityB))
                handEntity = triggerEvent.EntityB;

            if (interactiveObjects.HasComponent(triggerEvent.EntityA))
                interactiveEntity = triggerEvent.EntityA;
            else if (interactiveObjects.HasComponent(triggerEvent.EntityB))
                interactiveEntity = triggerEvent.EntityB;

            if (Entity.Null.Equals(handEntity)
                || Entity.Null.Equals(interactiveEntity)) return;

            //Init
            var hand = hands[handEntity];
            var interactiveObject = interactiveObjects[interactiveEntity];
            var handWorld = worldTransform[handEntity];
            var interactiveWorld = worldTransform[interactiveEntity];
            var isObjectInThisHand = (InHandType)(hand.handType + 1) == interactiveObject.inHand;

            //logic
            if(!hand.isReadyToTake) return;
            if (interactiveObject.inHand == InHandType.None)
                interactiveObject.distanceToHand = math.distance(handWorld.Position, interactiveWorld.Position);

            if ((hand.nearHand == Entity.Null ||
                 interactiveObjects[hand.nearHand].distanceToHand > interactiveObject.distanceToHand)
                && (interactiveObject.inHand == InHandType.None || isObjectInThisHand || interactiveObject.isSwitchHand))
            {
                hand.nearHand = interactiveEntity;
                hand.nextPose = interactiveObject.namePose;
            }

            //result
            hands[handEntity] = hand;
            interactiveObjects[interactiveEntity] = interactiveObject;
        }
    }
}