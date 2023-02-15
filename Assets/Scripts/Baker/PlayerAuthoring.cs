using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

public class PlayerAuthoring : MonoBehaviour
{
    public float speed = 1f;
    public float speedRotate = 1f;
    public bool isAlwaysRotate = true;
    public byte angleRotate;
}

public class PlayerBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        PlayerComponent player = default;
        player.speed = authoring.speed;
        player.speedRotate = authoring.speedRotate;
        player.isAlwaysRotate = authoring.isAlwaysRotate;
        player.angleRotate = authoring.angleRotate;
        AddComponent(player);
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

    public void OnEnable()
    {
        speed = serializedObject.FindProperty("speed");
        speedRotate = serializedObject.FindProperty("speedRotate");
        isAlwaysRotate = serializedObject.FindProperty("isAlwaysRotate");
        angleRotate = serializedObject.FindProperty("angleRotate");
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
        serializedObject.ApplyModifiedProperties();
    }
}
#endif