using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Components;
using Unity.Entities;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public partial class InputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var input = InputSingleton.Instance;
        
        if(input == null) {Debug.LogWarning("Input singleton not found"); return;}

        var camera = SystemAPI.GetSingleton<InputCamera>();
        var leftHand = SystemAPI.GetSingleton<InputLeftHand>();
        var rightHand = SystemAPI.GetSingleton<InputRightHand>();

        camera.position = input.GetHead.FindAction("Position").ReadValue<Vector3>();
        camera.rotation = input.GetHead.FindAction("Rotation").ReadValue<Quaternion>();

        leftHand.position = input.GetLeftHand.FindAction("Position").ReadValue<Vector3>();
        leftHand.rotation = input.GetLeftHand.FindAction("Rotation").ReadValue<Quaternion>();
        leftHand.trackingState = input.GetLeftHand.FindAction("Tracking State").ReadValue<int>();
        leftHand.grip = input.GetLeftHandInteraction.FindAction("Select").ReadValue<float>();
        leftHand.gripValue = input.GetLeftHandInteraction.FindAction("Select Value").ReadValue<float>();
        leftHand.trigger = input.GetLeftHandInteraction.FindAction("Activate").ReadValue<float>();
        leftHand.triggerValue = input.GetLeftHandInteraction.FindAction("Activate Value").ReadValue<float>();
        
        rightHand.position = input.GetRightHand.FindAction("Position").ReadValue<Vector3>();
        rightHand.rotation = input.GetRightHand.FindAction("Rotation").ReadValue<Quaternion>();
        rightHand.trackingState = input.GetRightHand.FindAction("Tracking State").ReadValue<int>();
        rightHand.grip = input.GetRightHandInteraction.FindAction("Select").ReadValue<float>();
        rightHand.gripValue = input.GetRightHandInteraction.FindAction("Select Value").ReadValue<float>();
        rightHand.trigger = input.GetRightHandInteraction.FindAction("Activate").ReadValue<float>();
        rightHand.triggerValue = input.GetRightHandInteraction.FindAction("Activate Value").ReadValue<float>();
        
        SystemAPI.SetSingleton(camera);
        SystemAPI.SetSingleton(leftHand);
        SystemAPI.SetSingleton(rightHand);
    }
}
