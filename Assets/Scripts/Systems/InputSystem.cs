using Components;
using SystemGroups;
using Tags;
using Unity.Entities;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

[UpdateInGroup(typeof(InputSystemGroup))]
public partial class InputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var input = InputSingleton.Instance;
        
        if(input == null) {Debug.LogWarning("Input singleton not found"); return;}

        // var camera = SystemAPI.GetSingletonRW<InputCamera>();
        //
        // camera.ValueRW.position = input.PositionPlayer + input.GetHead.FindAction("Position").ReadValue<Vector3>();
        // camera.ValueRW.rotation = input.GetHead.FindAction("Rotation").ReadValue<Quaternion>();

        foreach (var leftHand in SystemAPI.Query<RefRW<InputHand>>().WithAll<LeftHandTag>())
        {
            leftHand.ValueRW.position = input.GetLeftHand.FindAction("Position").ReadValue<Vector3>();
            leftHand.ValueRW.rotation = input.GetLeftHand.FindAction("Rotation").ReadValue<Quaternion>();
            leftHand.ValueRW.trackingState = input.GetLeftHand.FindAction("Tracking State").ReadValue<int>();
            leftHand.ValueRW.grip = input.GetLeftHandInteraction.FindAction("Select").ReadValue<float>();
            leftHand.ValueRW.gripValue = input.GetLeftHandInteraction.FindAction("Select Value").ReadValue<float>();
            leftHand.ValueRW.trigger = input.GetLeftHandInteraction.FindAction("Activate").ReadValue<float>();
            leftHand.ValueRW.triggerValue = input.GetLeftHandInteraction.FindAction("Activate Value").ReadValue<float>();
            leftHand.ValueRW.move = input.GetLeftHandLocomotion.FindAction("Move").ReadValue<Vector2>();
            input.PositionPlayer += new Vector3(leftHand.ValueRO.move.x, 0f, leftHand.ValueRO.move.y);
        }
        
        foreach (var rightHand in SystemAPI.Query<RefRW<InputHand>>().WithAll<RightHandTag>())
        {
            rightHand.ValueRW.position = input.GetRightHand.FindAction("Position").ReadValue<Vector3>();
            rightHand.ValueRW.rotation = input.GetRightHand.FindAction("Rotation").ReadValue<Quaternion>();
            rightHand.ValueRW.trackingState = input.GetRightHand.FindAction("Tracking State").ReadValue<int>();
            rightHand.ValueRW.grip = input.GetRightHandInteraction.FindAction("Select").ReadValue<float>();
            rightHand.ValueRW.gripValue = input.GetRightHandInteraction.FindAction("Select Value").ReadValue<float>();
            rightHand.ValueRW.trigger = input.GetRightHandInteraction.FindAction("Activate").ReadValue<float>();
            rightHand.ValueRW.triggerValue = input.GetRightHandInteraction.FindAction("Activate Value").ReadValue<float>();
            rightHand.ValueRW.move = input.GetRightHandLocomotion.FindAction("Move").ReadValue<Vector2>();
        }
        
        var player = SystemAPI.GetSingletonRW<PlayerComponent>();

        player.ValueRW.position = input.PositionPlayer;
    }
}
