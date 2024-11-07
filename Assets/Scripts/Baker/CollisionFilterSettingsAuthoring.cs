using Singletons;
using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEditor;
using UnityEngine;

namespace Baker
{
    public class CollisionFilterSettingsAuthoring : MonoBehaviour
    {
        
        public int itemGroupIndex = 0;
      
        public int otherGroupIndex = 0;
    }

    public class CollisionFilterSettingsBaker : Baker<CollisionFilterSettingsAuthoring>
    {
        public override void Bake(CollisionFilterSettingsAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            CollisionFilterSettings collisionFilterSettings = default;

            
            collisionFilterSettings.itemCollisionFilter.GroupIndex = authoring.itemGroupIndex;
            
            collisionFilterSettings.otherCollisionFilter.GroupIndex = authoring.otherGroupIndex;

            AddComponent(entity, collisionFilterSettings);
        }
    }

#if(UNITY_EDITOR)
    [CustomEditor(typeof(CollisionFilterSettingsAuthoring))]
    public class CollisionFilterSettingsEditor : Editor
    {
        private SerializedProperty _itemBelongTo;
        private SerializedProperty _itemCollidesWith;
        private SerializedProperty _itemGroupIndex;
        private SerializedProperty _otherBelongTo;
        private SerializedProperty _otherCollidesWith;
        private SerializedProperty _otherGroupIndex;

        private void OnEnable()
        {
            _itemBelongTo = serializedObject.FindProperty("itemBelongTo");
            _itemCollidesWith = serializedObject.FindProperty("itemCollidesWith");
            _itemGroupIndex = serializedObject.FindProperty("itemGroupIndex");
            _otherBelongTo = serializedObject.FindProperty("otherBelongTo");
            _otherCollidesWith = serializedObject.FindProperty("otherCollidesWith");
            _otherGroupIndex = serializedObject.FindProperty("otherGroupIndex");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Item");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_itemBelongTo);
            EditorGUILayout.PropertyField(_itemCollidesWith);
            EditorGUILayout.PropertyField(_itemGroupIndex);
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Other");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(_otherBelongTo);
            EditorGUILayout.PropertyField(_otherCollidesWith);
            EditorGUILayout.PropertyField(_otherGroupIndex);
            EditorGUI.indentLevel--;

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}