using System;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Scripts
{
    [Serializable]
    public struct HandPose
    {
        public Vector3 attachPosition;
        public Quaternion attachRotation;
        public List<Quaternion> fingerRotations;
    }
    
    [Serializable]
    public struct HandPoseData
    {
        public string name;
        public HandPose leftHand;
        public HandPose rightHand;
    }
    
    
    [Serializable]
    public struct HandInfo
    {
        public List<string> indents;
        public List<Transform> values;
        public List<bool> toggles;

        public HandInfo(bool isAutoInit = true)
        {
            if (isAutoInit)
            {
                indents = new List<string>();
                values = new List<Transform>();
                toggles = new List<bool>();
                return;
            }

            indents = null;
            values = null;
            toggles = null;
        }

        public void Clear()
        {
            indents = new List<string>();
            values = new List<Transform>();
            toggles = new List<bool>();
        }
    }

    [Serializable]
    public struct ColorFingers
    {
        public bool isForEachFinger;
        public List<Color32> bones;
        public List<Color32> joints;
    }

    [Serializable]
    public struct SaveDataSetting
    {
        public string path;
        public List<string> names;
        public List<MonoScript> templates;
    }

    [ExecuteInEditMode]
    public class HandPoseHelper : MonoBehaviour
    {
        public bool isSelectParentHand = true;
        [Space] [HideInInspector] public GameObject leftHandPrefab;
        [HideInInspector] public GameObject rightHandPrefab;
        [HideInInspector] [SerializeField] public List<Transform> leftHandJoints;
        [HideInInspector] [SerializeField] public List<Transform> rightHandJoints;

        [HideInInspector] [SerializeField] private GameObject _leftHand;
        [HideInInspector] [SerializeField] private GameObject _rightHand;

        [HideInInspector] public HandInfo leftHandInfo;
        [HideInInspector] public HandInfo rightHandInfo;

        [HideInInspector] public ColorFingers colorFingers;

        [HideInInspector] public int popupIndexTemplate;
        [HideInInspector] public SaveDataSetting saveDataSetting;

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

        public void OnEnable()
        {
            if (!leftHandPrefab || !rightHandPrefab)
            {
                Debug.LogError("Add prefab for Left or/and Right hand");
                return;
            }

            if (transform.childCount > 0)
            {
                _leftHand = transform.GetChild(0).gameObject;
                _rightHand = transform.GetChild(1).gameObject;
            }
            else
            {
                _leftHand = Instantiate(leftHandPrefab, Vector3.zero, Quaternion.identity, transform);
                _rightHand = Instantiate(rightHandPrefab, Vector3.zero, Quaternion.identity, transform);

                _leftHand.AddComponent<DrawGizmosHand>().joints = leftHandJoints;
                _rightHand.AddComponent<DrawGizmosHand>().joints = rightHandJoints;

                if (leftHandInfo.values.Count == 0)
                {
                    SetHandInfo(leftHandInfo, _leftHand.transform);
                    SetHandInfo(rightHandInfo, _rightHand.transform);
                }
                else
                {
                    leftHandInfo.values.Clear();
                    rightHandInfo.values.Clear();
                    RefreshTransformJoints(leftHandInfo, _leftHand.transform);
                    RefreshTransformJoints(rightHandInfo, _rightHand.transform);
                }
            }
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

        public void SetHandInfo(HandInfo handInfo, Transform transform, string space = "├",
            string line = "")
        {
            space = transform.GetSiblingIndex() == transform.parent?.childCount - 1
                ? space.Replace("├", "└")
                : space.Replace("└", "├");

            handInfo.indents.Add(transform.parent ? space : "    ");
            handInfo.values.Add(transform);
            handInfo.toggles.Add(false);

            if (!UseVerticalLine(transform, ref space, ref line))
                space = "      " + space;

            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                SetHandInfo(handInfo, child, space, line);
            }
        }

        private bool UseVerticalLine(Transform transform, ref string space, ref string line)
        {
            var isNewLine = transform.childCount > 0 && transform.GetSiblingIndex() < transform.parent?.childCount - 1;

            if (isNewLine || line.Length > 0)
            {
                line = (isNewLine) ? "│  " : "      ";

                if (space.Contains("└"))
                    space = space.Split("└")[0] + line + "└";
                else if (space.Contains("├"))
                    space = space.Split("├")[0] + line + "├";

                return true;
            }

            return false;
        }

        public void RefreshTransformJoints(HandInfo handInfo, Transform newValue)
        {
            handInfo.values.Add(newValue);

            for (var i = 0; i < newValue.childCount; i++)
            {
                var child = newValue.GetChild(i);
                RefreshTransformJoints(handInfo, child);
            }
        }

        public void RebuildJoints()
        {
            leftHandJoints.Clear();
            rightHandJoints.Clear();

            for (var i = 0; i < leftHandInfo.toggles.Count; i++)
            {
                if (leftHandInfo.toggles[i])
                    leftHandJoints.Add(leftHandInfo.values[i]);
            }

            for (var i = 0; i < rightHandInfo.toggles.Count; i++)
            {
                if (rightHandInfo.toggles[i])
                    rightHandJoints.Add(rightHandInfo.values[i]);
            }
        }

        public HandPoseData GetHandPoseData()
        {
            var handPoseData = new HandPoseData();

            handPoseData.leftHand.attachPosition = _offsetLeftHand;
            handPoseData.rightHand.attachPosition = _offsetRightHand;
            
            handPoseData.leftHand.fingerRotations = new List<Quaternion>();
            foreach (var transform in leftHandInfo.values)
            {
                handPoseData.leftHand.fingerRotations.Add(transform.rotation);
            }

            handPoseData.rightHand.fingerRotations = new List<Quaternion>();
            foreach (var transform in rightHandInfo.values)
            {
                handPoseData.rightHand.fingerRotations.Add(transform.rotation);
            }

            return handPoseData;
        }

        public void SetHandPoseData(HandPoseData handPoseData)
        {
            _offsetLeftHand = handPoseData.leftHand.attachPosition;
            _offsetRightHand = handPoseData.rightHand.attachPosition;

            for (var i = 0; i < handPoseData.leftHand.fingerRotations.Count; i++)
            {
                leftHandInfo.values[i].rotation = handPoseData.leftHand.fingerRotations[i];
            }
            
            for (var i = 0; i < handPoseData.rightHand.fingerRotations.Count; i++)
            {
                rightHandInfo.values[i].rotation = handPoseData.rightHand.fingerRotations[i];
            }
        }
    }

    [CustomEditor(typeof(HandPoseHelper))]
    public class HandPoseHelperEditor : Editor
    {
        private HandPoseHelper _handPoseHelper;

        private SerializedProperty _leftHandPrefab;
        private SerializedProperty _rightHandPrefab;

        private Transform _activeJoint;

        private bool _isFoldOutHandOptions;
        private bool _isFoldOutSaveDataOptions;
        private bool _isDisplayOneHand;
        private bool _isRootPrefab;

        private void OnEnable()
        {
            _handPoseHelper = (HandPoseHelper)target;

            if (!_handPoseHelper.leftHandPrefab || !_handPoseHelper.rightHandPrefab)
            {
                Debug.LogError("Add prefab for Left or/and Right hand");
                return;
            }

            _isRootPrefab = !PrefabUtility.GetCorrespondingObjectFromSource(_handPoseHelper.gameObject);

            serializedObject.Update();

            if (_isRootPrefab)
            {
                _leftHandPrefab = serializedObject.FindProperty("leftHandPrefab");
                _rightHandPrefab = serializedObject.FindProperty("rightHandPrefab");
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            _handPoseHelper = (HandPoseHelper)target;
            
            if (_isRootPrefab)
            {
                EditorGUILayout.PropertyField(_leftHandPrefab);
                EditorGUILayout.PropertyField(_rightHandPrefab);
                EditorGUILayout.Space();

                _isFoldOutSaveDataOptions =
                    EditorGUILayout.Foldout(_isFoldOutSaveDataOptions, "Setting Save Data", true);

                if (_isFoldOutSaveDataOptions)
                {
                    SettingSaveData();
                }

                serializedObject.ApplyModifiedProperties();
                return;
            }

            base.OnInspectorGUI();

            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            _isFoldOutHandOptions = EditorGUILayout.Foldout(_isFoldOutHandOptions, "Options fingers hand", true);
            _isDisplayOneHand = GUILayout.Toggle(_isDisplayOneHand, "2 in 1",
                GUILayout.Width(Screen.width - EditorGUIUtility.labelWidth - 25));
            GUILayout.EndHorizontal();

            if (_isFoldOutHandOptions)
            {
                _handPoseHelper.RebuildJoints();

                if (_isDisplayOneHand)
                {
                    Show2in1();
                }
                else
                {
                    ShowHierarchyHand(_handPoseHelper.leftHandInfo);
                    GUILayout.Space(10);
                    ShowHierarchyHand(_handPoseHelper.rightHandInfo);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void SettingSaveData()
        {
            var saveDataSetting = serializedObject.FindProperty("saveDataSetting");
            var names = saveDataSetting.FindPropertyRelative("names");
            var templates = saveDataSetting.FindPropertyRelative("templates");
            var path = saveDataSetting.FindPropertyRelative("path");

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(8);
            GUI.enabled = false;
            EditorGUILayout.TextField(path.stringValue);
            GUI.enabled = true;
            if (GUILayout.Button("New Save Path"))
            {
                string newPath = EditorUtility.OpenFolderPanel("Folder save data ", "", "");
                if (newPath.Contains("Assets"))
                    newPath = "Assets" + newPath.Split("Assets")[1];
                path.stringValue = newPath;
                return;
            }
            GUILayout.Space(22);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
            for (var i = 0; i < names.arraySize; i++)
            {
                var name = names.GetArrayElementAtIndex(i);
                var template = templates.GetArrayElementAtIndex(i);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(name, new GUIContent(""));
                EditorGUILayout.PropertyField(template, new GUIContent(""));
                if (GUILayout.Button("X"))
                {
                    names.DeleteArrayElementAtIndex(i);
                    templates.DeleteArrayElementAtIndex(i);
                    GUI.FocusControl(null);
                    break;
                }
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(8);
            if (GUILayout.Button("Add"))
            {
                names.arraySize++;
                templates.arraySize++;
            }
            GUILayout.Space(22);
            EditorGUILayout.EndHorizontal();
        }
        

        private bool AddElementHand(string indent, string name, bool toggle)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(indent, GUILayout.Width(GUI.skin.label.CalcSize(new GUIContent(indent)).x));
            var result = GUILayout.Toggle(toggle, name);
            GUILayout.EndHorizontal();

            return result;
        }

        private void ShowHierarchyHand(HandInfo handInfo)
        {
            for (var i = 0; i < handInfo.values.Count; i++)
            {
                var indent = handInfo.indents[i];
                var value = (Transform)handInfo.values[i];
                var toggle = handInfo.toggles[i];

                handInfo.toggles[i] = AddElementHand(indent, value.name, toggle);
            }
        }

        private void Show2in1()
        {
            if (_handPoseHelper.leftHandInfo.values.Count != _handPoseHelper.rightHandInfo.values.Count)
            {
                Debug.LogError("Left and Right hand have a different number of elements");
                return;
            }

            for (var i = 0; i < _handPoseHelper.leftHandInfo.values.Count; i++)
            {
                var indent = _handPoseHelper.leftHandInfo.indents[i];
                var toggle = _handPoseHelper.leftHandInfo.toggles[i] && _handPoseHelper.rightHandInfo.toggles[i];
                var name = _handPoseHelper.leftHandInfo.values[i].name == _handPoseHelper.rightHandInfo.values[i].name
                    ? _handPoseHelper.leftHandInfo.values[i].name
                    : _handPoseHelper.leftHandInfo.values[i].name + " | " +
                      _handPoseHelper.rightHandInfo.values[i].name;

                var temporaryToggle = AddElementHand(indent, name, toggle);

                _handPoseHelper.leftHandInfo.toggles[i] = temporaryToggle;
                _handPoseHelper.rightHandInfo.toggles[i] = temporaryToggle;
            }
        }
    }
}