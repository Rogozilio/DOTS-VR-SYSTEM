using Components;
using Enums;
using SystemGroups;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(AnimationSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(AnimationSystem))]
    public partial class AnimationReturnToGO : SystemBase
    {
        private FixedList512Bytes<quaternion> _leftHand;
        private FixedList512Bytes<quaternion> _rightHand;
        
        protected override void OnUpdate()
        {
            var hands = HandsSingleton.Instance;

            if (hands == null)
            {
                Debug.LogError("Hands singleton not found");
                return;
            }

            foreach (var hand in SystemAPI.Query<RefRO<Hand>>())
            {
                if(hand.ValueRO.handType == HandType.Left)
                    _leftHand = hand.ValueRO.joints;
                else
                    _rightHand = hand.ValueRO.joints;
            }

            hands.SetRotateHands(_leftHand, _rightHand);
        }
    }
}