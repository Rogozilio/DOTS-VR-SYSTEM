using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Scripts
{
    [ExecuteInEditMode]
    public class HandPoseHelper : MonoBehaviour
    {
        public bool isSelectParentHand = true;
        [Space] public GameObject leftHandPrefab;
        public GameObject rightHandPrefab;
        [Space] [HideInInspector] public List<Transform> joints;

        private GameObject _leftHand;
        private GameObject _rightHand;

        private Vector3 _offsetLeftHand;
        private Vector3 _offsetRightHand;

        private GameObject _mainSelectObject;


        private bool IsVisibleHands
        {
            set
            {
                _leftHand.SetActive(value);
                _rightHand.SetActive(value);
            }
            get => _leftHand.activeSelf && _rightHand.activeSelf;
        }

        private GameObject SetMainSelectObject
        {
            set
            {
                if (value != _mainSelectObject &&
                    value != _leftHand &&
                    value != _rightHand)
                    _mainSelectObject = value;
            }
        }

        private void OnEnable()
        {
            _leftHand = Instantiate(leftHandPrefab, Vector3.zero, Quaternion.identity, transform);
            _rightHand = Instantiate(rightHandPrefab, Vector3.zero, Quaternion.identity, transform);
        }

        private void Update()
        {
            if (!Selection.activeGameObject)
            {
                IsVisibleHands = false;
                return;
            }

            var selectObj = Selection.activeGameObject;

            if (isSelectParentHand) selectObj = SelectParentHand(selectObj);

            IsVisibleHands = true;
            SetMainSelectObject = selectObj;
            SetOffsetHands(selectObj);
            SetPositionHands(selectObj);
        }

        private void OnDisable()
        {
            if (_leftHand) DestroyImmediate(_leftHand);
            if (_rightHand) DestroyImmediate(_rightHand);
        }

        private void SetOffsetHands(GameObject selectObj)
        {
            if (selectObj == _leftHand)
                _offsetLeftHand = _leftHand.transform.position - _mainSelectObject.transform.position;
            else if (selectObj == _rightHand)
                _offsetRightHand = _rightHand.transform.position - _mainSelectObject.transform.position;
        }

        private void SetPositionHands(GameObject selectObj)
        {
            var selectPosition = selectObj.transform.position;

            if (selectObj == _leftHand)
                _leftHand.transform.position = selectPosition;
            else if (selectObj == _rightHand)
                _rightHand.transform.position = selectPosition;
            else
            {
                _leftHand.transform.position = selectPosition + _offsetLeftHand;
                _rightHand.transform.position = selectPosition + _offsetRightHand;
            }
        }

        private GameObject SelectParentHand(GameObject selectObject)
        {
            var parent = selectObject.transform.parent;

            if (!parent) return Selection.activeGameObject;

            if (selectObject != _leftHand && selectObject != _rightHand)
            {
                Selection.activeObject = SelectParentHand(parent.gameObject);
            }
            else
            {
                return selectObject;
            }

            return Selection.activeGameObject;
        }
    }

    [CustomEditor(typeof(HandPoseHelper))]
    public class HandPoseHelperEditor : Editor
    {
        private List<string> _space;
        private List<string> _name;
        private List<bool> _toggles;

        private void OnEnable()
        {
            var handPoseHelper = (HandPoseHelper)target;

            _space = new List<string>();
            _name = new List<string>();
            _toggles = new List<bool>();

            if (!handPoseHelper.leftHandPrefab || !handPoseHelper.rightHandPrefab)
            {
                Debug.LogError("Add prefab for Left or/and Right hand");
                return;
            }

            GetAllChild(handPoseHelper.leftHandPrefab.transform);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            for (var i = 0; i < _name.Count; i++)
            {
                GUI.skin.label.CalcSize(new GUIContent("text"));
                GUILayout.BeginHorizontal();
                //GUILayout.Space(_space[i] - 6);
                GUILayout.Label("└", GUILayout.Width(GUI.skin.label.CalcSize(new GUIContent(_space[i])).x));
                //GUILayout.Label("│", GUILayout.Width(15f));
                _toggles[i] = GUILayout.Toggle(_toggles[i], _name[i]);
                GUILayout.EndHorizontal();
            }
        }

        private void GetAllChild(Transform transform, string space = "└")
        {
            _space.Add(space);
            _name.Add(transform.name);
            _toggles.Add(false);
            
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                GetAllChild(child, space + " ");
            }
        }

        // private void OnSceneGUI()
        // {
        //     DrawJointButtons();
        //     DrawJointHandle();
        // }
        //
        // private void DrawJointButtons()
        // {
        //     // Draw a button for each joint
        //     foreach (Transform joint in previewHand.Joints)
        //     {
        //         // Were one of the buttons pressed?
        //         bool pressed = Handles.Button(joint.position, joint.rotation, 0.01f, 0.005f, Handles.SphereHandleCap);
        //
        //         // Did we select the same joint?
        //         if (pressed)
        //             activeJoint = IsSelected(joint) ? null : joint;                
        //     }
        // }
        //
        // private bool IsSelected(Transform joint)
        // {
        //     return joint == activeJoint;
        // }
        //
        // private void DrawJointHandle()
        // {
        //     // If a joint is selected
        //     if(HasActiveJoint())
        //     {
        //         // Draw handle
        //         Quaternion currentRotation = activeJoint.rotation;
        //         Quaternion newRotation = Handles.RotationHandle(currentRotation, activeJoint.position);
        //
        //         // Detect if handle has rotated
        //         if (HandleRotated(currentRotation, newRotation))
        //         {
        //             activeJoint.rotation = newRotation;
        //             Undo.RecordObject(target, "Joint Rotated");
        //         }
        //     }
        // }
        //
        // private bool HasActiveJoint()
        // {
        //     return activeJoint;
        // }
        //
        // private bool HandleRotated(Quaternion currentRotation, Quaternion newRotation)
        // {
        //     return currentRotation != newRotation;
        // }
    }
}