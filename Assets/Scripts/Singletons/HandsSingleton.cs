using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class HandsSingleton : MonoBehaviour
{
    public static HandsSingleton Instance;

    [SerializeField] private Transform _leftHand;
    [SerializeField] private Transform _rightHand;

    private List<Transform> _jointsLeftHand;
    private List<Transform> _jointsRightHand;

    public float3 SetPositionLeftHand
    {
        set => _leftHand.position = value;
    }
    public float3 SetPositionRightHand
    {
        set => _rightHand.position = value;
    }
    public quaternion SetRotationLeftHand
    {
        set => _leftHand.rotation = value;
    }
    public quaternion SetRotationRightHand
    {
        set => _rightHand.rotation = value;
    }
    public List<quaternion> GetRotateLeftHand() => GetRotateHand();
    public List<quaternion> GetRotateRightHand() => GetRotateHand(false);
    private List<quaternion> GetRotateHand(bool isLeftHand = true)
    {
        var result = new List<quaternion>();

        foreach (var joint in isLeftHand ? _jointsLeftHand : _jointsRightHand)
        {
            result.Add(joint.localRotation);
        }

        return result;
    }
    
    public void SetRotateHands(FixedList512Bytes<quaternion> newLeftRotate, FixedList512Bytes<quaternion> newRightRotate)
    {
        var isJointsLeftHandMatch = _jointsLeftHand.Count == newLeftRotate.Length;
        var isJointsRightHandMatch = _jointsRightHand.Count == newRightRotate.Length;
        var isJointsHandsMatch = isJointsLeftHandMatch && isJointsRightHandMatch &&
                                 newLeftRotate.Length == newRightRotate.Length;

        if (isJointsHandsMatch)
        {
            for (var i = 0; i < _jointsLeftHand.Count; i++)
            {
                _jointsLeftHand[i].localRotation = newLeftRotate[i];
                _jointsRightHand[i].localRotation = newRightRotate[i];
            }

            return;
        }

        if (isJointsLeftHandMatch && isJointsRightHandMatch)
        {
            for (var i = 0; i < _jointsLeftHand.Count; i++)
            {
                _jointsLeftHand[i].localRotation = newLeftRotate[i];
            }

            for (var i = 0; i < _jointsRightHand.Count; i++)
            {
                _jointsRightHand[i].localRotation = newRightRotate[i];
            }
        }
        else
        {
            Debug.LogError("Joints left or right hand not match. Joints left hand => " + _jointsLeftHand.Count +
                           " != " +
                           newLeftRotate.Length + ". Joints right hand => " + _jointsRightHand.Count + " != " +
                           newRightRotate.Length);
        }
    }

    private void GetAllJoint(ref List<Transform> joints, Transform hand)
    {
        foreach (Transform child in hand)
        {
            joints.Add(child);
            GetAllJoint(ref joints, child);
        }
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        _jointsLeftHand = new List<Transform>();
        _jointsRightHand = new List<Transform>();

        _jointsLeftHand.Add(_leftHand);
        _jointsRightHand.Add(_rightHand);

        GetAllJoint(ref _jointsLeftHand, _leftHand);
        GetAllJoint(ref _jointsRightHand, _rightHand);
        Instance = this;
    }
}