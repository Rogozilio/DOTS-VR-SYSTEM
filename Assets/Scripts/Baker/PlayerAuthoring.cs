using System;
using Components;
using Enums;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAuthoring : MonoBehaviour
{
    public float speed = 1f;
    public float speedRotate = 1f;
    public bool isAlwaysRotate = true;
    public byte angleRotate;
    public DeltaType deltaType;
}

public class PlayerBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
        PlayerComponent player = default;
        player.speed = authoring.speed;
        player.speedRotate = authoring.speedRotate;
        player.isAlwaysRotate = authoring.isAlwaysRotate;
        player.angleRotate = authoring.angleRotate;
        player.deltaType = authoring.deltaType;
        AddComponent(entity, player);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlayerAuthoring))]
public class PlayerAuthoringEditor : Editor
{
    private SerializedProperty speed;
    private SerializedProperty speedRotate;
    private SerializedProperty isAlwaysRotate;
    private SerializedProperty angleRotate;
    private SerializedProperty deltaSmoothLerp;

    public void OnEnable()
    {
        speed = serializedObject.FindProperty("speed");
        speedRotate = serializedObject.FindProperty("speedRotate");
        isAlwaysRotate = serializedObject.FindProperty("isAlwaysRotate");
        angleRotate = serializedObject.FindProperty("angleRotate");
        deltaSmoothLerp = serializedObject.FindProperty("deltaType");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(speed);
        EditorGUILayout.PropertyField(isAlwaysRotate);
        EditorGUI.indentLevel++;
        if (isAlwaysRotate.boolValue)
            EditorGUILayout.PropertyField(speedRotate);
        else
            EditorGUILayout.IntSlider(angleRotate, 1, 180);
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(deltaSmoothLerp);
        
        serializedObject.ApplyModifiedProperties();
    }
}
#endif