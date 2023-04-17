using System;
using System.Collections.Generic;
using UnityEngine;
using Components;
using Enums;
using Unity.Entities;
using UnityEditor;

namespace Baker
{
    public class InteractiveAuthoring : MonoBehaviour
    {
        public string namePose;
        [Space] public HandActionType handActionType;
        public InteractiveType interactiveType;

        [Space] [Header("Smooth object")] [Range(0, 1)]
        public float beginValueSmooth = 0.3f;
    }

    public class InteractiveBaker : Baker<InteractiveAuthoring>
    {
        public override void Bake(InteractiveAuthoring authoring)
        {
            InteractiveObject interactiveObject = default;
            interactiveObject.namePose = authoring.namePose;
            interactiveObject.handActionType = authoring.handActionType;
            interactiveObject.interactiveType = authoring.interactiveType;
            interactiveObject.beginValueSmooth = authoring.beginValueSmooth;
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity, interactiveObject);
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(InteractiveAuthoring))]
    public class InteractiveAuthoringEditor : Editor
    {
        private List<string> _names;
        private int _selectedIndex;

        private SerializedProperty _namePose;
        private SerializedProperty _handActionType;
        private SerializedProperty _interactiveType;
        private SerializedProperty _beginValueSmooth;

        public void OnEnable()
        {
            _names = new List<string>();

            _namePose = serializedObject.FindProperty("namePose");
            _handActionType = serializedObject.FindProperty("handActionType");
            _interactiveType = serializedObject.FindProperty("interactiveType");
            _beginValueSmooth = serializedObject.FindProperty("beginValueSmooth");

            _selectedIndex = FindIndexName(_namePose.stringValue);
        }

        public override void OnInspectorGUI()
        {
            _names.Clear();

            BrowseNames((nameFile, namePose, index) => { _names.Add(nameFile + " / " + namePose); });

            _selectedIndex = EditorGUILayout.Popup("Name Pose", _selectedIndex, _names.ToArray());
            _namePose.stringValue = _names[_selectedIndex].Split("/ ")[1];

            EditorGUILayout.PropertyField(_handActionType);
            EditorGUILayout.PropertyField(_interactiveType);
            EditorGUILayout.Space();
            _beginValueSmooth.floatValue = EditorGUILayout.Slider("BeginValueSmooth", _beginValueSmooth.floatValue, 0f, 1f);

            serializedObject.ApplyModifiedProperties();
        }

        private int FindIndexName(string name)
        {
            var selectedIndex = -1;
            BrowseNames((nameFile, namePose, index) =>
            {
                if (namePose == name) selectedIndex = index;
            });

            return selectedIndex;
        }

        private void BrowseNames(Action<string, string, int> action)
        {
            var index = 0;
            var guids = AssetDatabase.FindAssets("t:SaveDataTemplate", null);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var defaultListTemplate =
                    (DefaultListTemplate)AssetDatabase.LoadAssetAtPath(path, typeof(DefaultListTemplate));
                foreach (var name in defaultListTemplate.GetAllNames)
                {
                    action?.Invoke(defaultListTemplate.name, name, index++);
                }
            }
        }
    }
#endif
}