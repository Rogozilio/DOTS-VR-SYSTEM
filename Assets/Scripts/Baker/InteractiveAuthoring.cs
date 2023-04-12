using UnityEngine;
using Components;
using Unity.Entities;
using UnityEditor;

namespace Baker
{
    public class InteractiveAuthoring : MonoBehaviour
    {
        public string namePose;
        [Space] public bool isSwitchHand = true;
        [Space][Header("Smooth object")] [Range(0, 1)]
        public float beginValueSmooth = 0.3f;
    }

    public class InteractiveBaker : Baker<InteractiveAuthoring>
    {
        public override void Bake(InteractiveAuthoring authoring)
        {
            InteractiveObject interactiveObject = default;
            interactiveObject.namePose = authoring.namePose;
            interactiveObject.isSwitchHand = authoring.isSwitchHand;
            interactiveObject.beginValueSmooth = authoring.beginValueSmooth;
            Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
            AddComponent(entity, interactiveObject);
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