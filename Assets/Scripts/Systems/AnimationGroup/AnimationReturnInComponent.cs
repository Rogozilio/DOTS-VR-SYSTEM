using System.Collections.Generic;
using Components;
using Enums;
using SystemGroups;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(AnimationSystemGroup), OrderFirst = true)]
public partial class AnimationReturnInComponent : SystemBase
{
    protected override void OnUpdate()
    {
        var hands = HandsSingleton.Instance;

        if (hands == null)
        {
            Debug.LogError("Hands singleton not found");
            return;
        }

        foreach (var hand in SystemAPI.Query<RefRW<Hand>>())
        {
            var handRotate = new List<quaternion>();
            handRotate = hand.ValueRO.handType == HandType.Left 
                ? hands.GetRotateLeftHand() : hands.GetRotateRightHand();

            for (var i = 0; i < hand.ValueRO.joints.Length; i++)
            {
                hand.ValueRW.joints[i] = handRotate[i];
            }
        }
    }
}