using Components;
using Enums;
using SystemGroups;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

[UpdateInGroup(typeof(InputSystemGroup))]
public partial class InputSystem : SystemBase
{
    protected override void OnStartRunning()
    {
        var input = InputSingleton.Instance;

        if (input == null)
        {
            Debug.LogError("Input singleton not found");
            return;
        }

        var player = SystemAPI.GetSingletonRW<PlayerComponent>();

        player.ValueRW.nextPosition = input.PositionPlayer;
    }

    protected override void OnUpdate()
    {
        var input = InputSingleton.Instance;

        if (input == null)
        {
            Debug.LogError("Input singleton not found");
            return;
        }

        var player = SystemAPI.GetSingletonRW<PlayerComponent>();

        // var camera = SystemAPI.GetSingletonRW<InputCamera>();
        //
        // camera.ValueRW.position = input.PositionPlayer + input.GetHead.FindAction("Position").ReadValue<Vector3>();
        // camera.ValueRW.rotation = input.GetHead.FindAction("Rotation").ReadValue<Quaternion>();

        foreach (var (inputHand, hand) in SystemAPI.Query<RefRW<InputHand>, RefRO<Hand>>())
        {
            if (hand.ValueRO.handType == HandType.Left)
            {
                inputHand.ValueRW.position = input.GetLeftHand.FindAction("Position").ReadValue<Vector3>();
                inputHand.ValueRW.rotation = input.GetLeftHand.FindAction("Rotation").inProgress
                    ? input.GetLeftHand.FindAction("Rotation").ReadValue<Quaternion>()
                    : quaternion.identity;
                inputHand.ValueRW.trackingState = input.GetLeftHand.FindAction("Tracking State").ReadValue<int>();
                inputHand.ValueRW.grip = input.GetLeftHandInteraction.FindAction("Select").ReadValue<float>();
                inputHand.ValueRW.gripValue =
                    input.GetLeftHandInteraction.FindAction("Select Value").ReadValue<float>();
                inputHand.ValueRW.trigger = input.GetLeftHandInteraction.FindAction("Activate").ReadValue<float>();
                inputHand.ValueRW.triggerValue =
                    input.GetLeftHandInteraction.FindAction("Activate Value").ReadValue<float>();
                inputHand.ValueRW.move = input.GetLeftHandLocomotion.FindAction("Move").ReadValue<Vector2>();
                inputHand.ValueRW.turn = input.GetLeftHandLocomotion.FindAction("Turn").ReadValue<Vector2>();

                player.ValueRW.velocity = new Vector3(inputHand.ValueRO.move.x, 0f, inputHand.ValueRO.move.y);
            }
            else
            {
                inputHand.ValueRW.position = input.GetRightHand.FindAction("Position").ReadValue<Vector3>();
                inputHand.ValueRW.rotation = input.GetRightHand.FindAction("Rotation").inProgress
                    ? input.GetRightHand.FindAction("Rotation").ReadValue<Quaternion>()
                    : quaternion.identity;
                inputHand.ValueRW.trackingState = input.GetRightHand.FindAction("Tracking State").ReadValue<int>();
                inputHand.ValueRW.grip = input.GetRightHandInteraction.FindAction("Select").ReadValue<float>();
                inputHand.ValueRW.gripValue =
                    input.GetRightHandInteraction.FindAction("Select Value").ReadValue<float>();
                inputHand.ValueRW.trigger = input.GetRightHandInteraction.FindAction("Activate").ReadValue<float>();
                inputHand.ValueRW.triggerValue =
                    input.GetRightHandInteraction.FindAction("Activate Value").ReadValue<float>();
                inputHand.ValueRW.move = input.GetRightHandLocomotion.FindAction("Move").ReadValue<Vector2>();
                inputHand.ValueRW.turn = input.GetRightHandLocomotion.FindAction("Turn").ReadValue<Vector2>();
                inputHand.ValueRW.isTurnOnce = input.GetRightHandLocomotion.FindAction("TurnOnce").triggered;

                player.ValueRW.angularVelocity = inputHand.ValueRO.turn.x;
                player.ValueRW.isTurnOnce = inputHand.ValueRO.isTurnOnce;
                switch ((int)math.sign(player.ValueRW.angularVelocity))
                {
                    case -1:
                        player.ValueRW.angleRotate = math.min(-player.ValueRO.angleRotate, player.ValueRO.angleRotate);
                        break;
                    case 1:
                        player.ValueRW.angleRotate = math.max(-player.ValueRO.angleRotate, player.ValueRO.angleRotate);
                        break;
                }
            }
        }

        MovePlayer(input, player);
        RotatePlayer(input, player);
    }

    private void MovePlayer(InputSingleton input, RefRW<PlayerComponent> player)
    {
        if (player.ValueRO.velocity.z == 0) return;

        var velocityForward = new float3(input.GetForwardCamera.x, 0, input.GetForwardCamera.z) *
                              player.ValueRO.velocity.z;
        var velocityRight = new float3(input.GetRightCamera.x, 0, input.GetRightCamera.z) * player.ValueRO.velocity.x;
        var velocity = (math.normalize(velocityForward + velocityRight)) * player.ValueRO.speed *
                       SystemAPI.Time.DeltaTime;

        player.ValueRW.nextPosition = input.PositionPlayer + velocity;
        input.PositionPlayer = player.ValueRO.nextPosition;
    }

    private void RotatePlayer(InputSingleton input, RefRW<PlayerComponent> player)
    {
        var speed = player.ValueRO.angularVelocity * player.ValueRO.speedRotate * SystemAPI.Time.DeltaTime;
        var isOnceRotate = !player.ValueRO.isAlwaysRotate && player.ValueRO.isTurnOnce;
        var angle = isOnceRotate ? player.ValueRO.angleRotate : 0f;
        var deltaRotate = quaternion.RotateY(player.ValueRO.isAlwaysRotate ? speed : math.radians(angle));

        player.ValueRW.nextRotation = math.mul(input.RotationPlayer, deltaRotate);
        input.RotationPlayer = player.ValueRO.nextRotation;
    }
}