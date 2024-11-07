using System;
using Components;
using Tags;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public InputActionAsset inputAction;

    private InputActionMap _head;
    private InputActionMap _left;
    private InputActionMap _leftInteraction;
    private InputActionMap _leftLocomotion;
    private InputActionMap _right;
    private InputActionMap _rightInteraction;
    private InputActionMap _rightLocomotion;

    private EntityManager _entityManager;
    private EntityQuery _entityCamera;
    private EntityQuery _entityLeftHand;
    private EntityQuery _entityRightHand;
    private Entity _camera;
    private Entity _leftHand;
    private Entity _rightHand;

    private void Awake()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        _head = inputAction.FindActionMap("XRI Head");
        _left = inputAction.FindActionMap("XRI Left");
        _leftInteraction = inputAction.FindActionMap("XRI Left Interaction");
        _leftLocomotion = inputAction.FindActionMap("XRI Left Locomotion");
        _right = inputAction.FindActionMap("XRI Right");
        _rightInteraction = inputAction.FindActionMap("XRI Right Interaction");
        _rightLocomotion = inputAction.FindActionMap("XRI Right Locomotion");
    }

    private void Start()
    {
        _entityCamera = _entityManager.CreateEntityQuery(typeof(InputCamera));
        _entityLeftHand = _entityManager.CreateEntityQuery(typeof(LeftHandTag));
        _entityRightHand = _entityManager.CreateEntityQuery(typeof(RightHandTag));
        
        _camera = _entityCamera.GetSingletonEntity();
        _leftHand = _entityLeftHand.GetSingletonEntity();
        _rightHand = _entityRightHand.GetSingletonEntity();
    }

    private void OnEnable()
    {
        inputAction?.Enable();
    }

    private void OnDisable()
    {
        inputAction?.Disable();
    }

    private void Update()
    {
        var camera = _entityManager.GetComponentData<InputCamera>(_camera);
        var leftHand = _entityManager.GetComponentData<InputHand>(_leftHand);
        var rightHand = _entityManager.GetComponentData<InputHand>(_rightHand);

        camera.position = _head.FindAction("Position").ReadValue<Vector3>();
        camera.rotation = _head.FindAction("Rotation").ReadValue<Quaternion>();
        
        leftHand.position = _left.FindAction("Position").ReadValue<Vector3>();
        leftHand.rotation = _left.FindAction("Rotation").inProgress
            ? _left.FindAction("Rotation").ReadValue<Quaternion>()
            : quaternion.identity;
        leftHand.trackingState = _left.FindAction("Tracking State").ReadValue<int>();
        leftHand.grip = _leftInteraction.FindAction("Select").ReadValue<float>();
        leftHand.gripValue = _leftInteraction.FindAction("Select Value").ReadValue<float>();
        leftHand.trigger = _leftInteraction.FindAction("Activate").ReadValue<float>();
        leftHand.triggerValue = _leftInteraction.FindAction("Activate Value").ReadValue<float>();
        leftHand.move = _leftLocomotion.FindAction("Move").ReadValue<Vector2>();
        leftHand.turn = _leftLocomotion.FindAction("Turn").ReadValue<Vector2>();
        
        rightHand.position = _right.FindAction("Position").ReadValue<Vector3>();
        rightHand.rotation = _right.FindAction("Rotation").inProgress
            ? _right.FindAction("Rotation").ReadValue<Quaternion>()
            : quaternion.identity;
        rightHand.trackingState = _right.FindAction("Tracking State").ReadValue<int>();
        rightHand.grip = _rightInteraction.FindAction("Select").ReadValue<float>();
        rightHand.gripValue = _rightInteraction.FindAction("Select Value").ReadValue<float>();
        rightHand.trigger = _rightInteraction.FindAction("Activate").ReadValue<float>();
        rightHand.triggerValue = _rightInteraction.FindAction("Activate Value").ReadValue<float>();
        rightHand.move = _rightLocomotion.FindAction("Move").ReadValue<Vector2>();
        rightHand.turn = _rightLocomotion.FindAction("Turn").ReadValue<Vector2>();
        
        _entityManager.SetComponentData(_camera, camera);
        _entityManager.SetComponentData(_leftHand, leftHand);
        _entityManager.SetComponentData(_rightHand, rightHand);
    }
    
    // private void MovePlayer(InputSingleton input, RefRW<PlayerComponent> player)
    // {
    //     if (player.ValueRO.velocity.z == 0) return;
    //
    //     var velocityForward = new float3(input.GetForwardCamera.x, 0, input.GetForwardCamera.z) *
    //                           player.ValueRO.velocity.z;
    //     var velocityRight = new float3(input.GetRightCamera.x, 0, input.GetRightCamera.z) * player.ValueRO.velocity.x;
    //     var velocity = (math.normalize(velocityForward + velocityRight)) * player.ValueRO.speed *
    //                    SystemAPI.Time.DeltaTime;
    //
    //     player.ValueRW.nextPosition = input.PositionPlayer + velocity;
    //     input.PositionPlayer = player.ValueRO.nextPosition;
    // }
    //
    // private void RotatePlayer(InputSingleton input, RefRW<PlayerComponent> player)
    // {
    //     var speed = player.ValueRO.angularVelocity * player.ValueRO.speedRotate * SystemAPI.Time.DeltaTime;
    //     var isOnceRotate = !player.ValueRO.isAlwaysRotate && player.ValueRO.isTurnOnce;
    //     var angle = isOnceRotate ? player.ValueRO.angleRotate : 0f;
    //     var deltaRotate = quaternion.RotateY(player.ValueRO.isAlwaysRotate ? speed : math.radians(angle));
    //
    //     player.ValueRW.nextRotation = math.mul(input.RotationPlayer, deltaRotate);
    //     input.RotationPlayer = player.ValueRO.nextRotation;
    // }
}