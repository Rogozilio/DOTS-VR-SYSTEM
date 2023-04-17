using Aspects;
using Enums;
using SystemGroups;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(InputSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(PlayerMoveSystem))]
    public partial class InputReturnInGO : SystemBase
    {
        protected override void OnUpdate()
        {
            var hands = HandsSingleton.Instance;

            if (hands == null)
            {
                Debug.LogError("Hands singleton not found");
                return;
            }

            foreach (var hand in SystemAPI.Query<HandAspect>())
            {
                if (hand.GetHandType == HandType.Left)
                {
                    hands.SetPositionLeftHand = hand.localTransform.ValueRO.Position;
                    hands.SetRotationLeftHand =
                        math.mul(hand.localTransform.ValueRO.Rotation, hand.GetDefaultPoseRotation);
                }
                else
                {
                    hands.SetPositionRightHand = hand.localTransform.ValueRO.Position;
                    hands.SetRotationRightHand =
                        math.mul(hand.localTransform.ValueRO.Rotation, hand.GetDefaultPoseRotation);
                }
            }
        }
    }
}