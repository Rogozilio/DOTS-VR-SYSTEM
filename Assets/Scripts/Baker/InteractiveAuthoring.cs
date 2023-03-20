using UnityEngine;
using Components;
using Unity.Entities;
using UnityEditor;

namespace Baker
{
    public class InteractiveAuthoring : MonoBehaviour
    {
        public string namePose;
    }

    public class InteractiveBaker : Baker<InteractiveAuthoring>
    {
        public override void Bake(InteractiveAuthoring authoring)
        {
            InteractiveObject interactiveObject = default;
            interactiveObject.namePose = authoring.namePose;
            AddComponent(interactiveObject);
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(InteractiveAuthoring))]
    public class InteractiveAuthoringEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            string[] guids = AssetDatabase.FindAssets ("t:SaveDataTemplate", null);
            foreach (string guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asd = (DefaultListTemplate)AssetDatabase.LoadAssetAtPath(path, typeof(DefaultListTemplate));
            }
            
            base.OnInspectorGUI();
        }
    }
#endif
}