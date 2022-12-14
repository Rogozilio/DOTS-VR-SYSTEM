using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSingleton : MonoBehaviour
{
    public static InputSingleton Instance;

    [SerializeField] private InputActionAsset _inputActionAsset;

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
