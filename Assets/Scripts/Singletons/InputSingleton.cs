using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSingleton : MonoBehaviour
{
    public static InputSingleton Instance;

    [SerializeField] private Transform _player;
    [SerializeField] private Transform _camera;
    [SerializeField] private InputActionAsset _inputActionAsset;

    public Vector3 PositionPlayer
    {
        set => _player.position = value;
        get => _player.position;
    }

    public Vector3 GetForwardCamera => _camera.forward;
    public Vector3 GetRightCamera => _camera.right;
    public InputActionMap GetHead => _inputActionAsset.FindActionMap("XRI Head");
    public InputActionMap GetLeftHand => _inputActionAsset.FindActionMap("XRI LeftHand");
    public InputActionMap GetLeftHandInteraction => _inputActionAsset.FindActionMap("XRI LeftHand Interaction");
    public InputActionMap GetLeftHandLocomotion => _inputActionAsset.FindActionMap("XRI LeftHand Locomotion");
    public InputActionMap GetRightHand => _inputActionAsset.FindActionMap("XRI RightHand");
    public InputActionMap GetRightHandInteraction => _inputActionAsset.FindActionMap("XRI RightHand Interaction");
    public InputActionMap GetRightHandLocomotion => _inputActionAsset.FindActionMap("XRI RightHand Locomotion");

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
