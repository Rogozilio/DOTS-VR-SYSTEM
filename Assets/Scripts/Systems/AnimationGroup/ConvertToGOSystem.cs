using System.Collections.Generic;
using Components;
using SystemGroups;
using Tags;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Systems
{
    [UpdateInGroup(typeof(AnimationSystemGroup), OrderLast = true)]
    [UpdateAfter(typeof(AnimationSystem))]
    public partial class ConvertToGOSystem : SystemBase
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

            foreach (var leftHand in SystemAPI.Query<RefRO<Hand>>().WithAll<LeftHandTag>())
            {
                _leftHand = leftHand.ValueRO.joints;
            }

            foreach (var rightHand in SystemAPI.Query<RefRO<Hand>>().WithAll<RightHandTag>())
            {
                _rightHand = rightHand.ValueRO.joints;
            }

            hands.SetRotateHands(_leftHand, _rightHand);
        }
    }
}