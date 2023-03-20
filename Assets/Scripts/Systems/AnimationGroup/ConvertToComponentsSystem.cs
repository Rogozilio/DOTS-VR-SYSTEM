using Components;
using SystemGroups;
using Tags;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(AnimationSystemGroup), OrderFirst = true)]
public partial class ConvertToComponentsSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var hands = HandsSingleton.Instance;

        if (hands == null)
        {
            Debug.LogError("Hands singleton not found");
            return;
        }

        foreach (var leftHand in SystemAPI.Query<RefRW<Hand>>().WithAll<LeftHandTag>())
        {
            var leftHandRotate = hands.GetRotateLeftHand();

            for (var i = 0; i < leftHand.ValueRO.joints.Length; i++)
            {
                leftHand.ValueRW.joints[i] = leftHandRotate[i];
            }
        }
        
        foreach (var rightHand in SystemAPI.Query<RefRW<Hand>>().WithAll<RightHandTag>())
        {
            var rightHandRotate = hands.GetRotateRightHand();

            for (var i = 0; i < rightHand.ValueRO.joints.Length; i++)
            {
                rightHand.ValueRW.joints[i] = rightHandRotate[i];
            }
        }
    }
}