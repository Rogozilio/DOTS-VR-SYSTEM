using System;
using System.Collections;
using System.Collections.Generic;
using Scripts;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), "Hand Pose")]
public class HandPoseOverlay : IMGUIOverlay
{
    private static List<HandPoseOverlay> instances = new List<HandPoseOverlay>();

    private bool _isShowColorFingers;

    private Texture2D _iconBone;
    private Texture2D _iconJoint;

    public override void OnCreated()
    {
        instances.Add(this);

        _iconBone = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Sprites/bone.png");
        _iconJoint = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Editor/Sprites/joint.png");
    }

    public override void OnWillBeDestroyed()
    {
        instances.Remove(this);
    }

    public static void DoWithInstances(Action<HandPoseOverlay> doWithInstance)
    {
        foreach (var instance in instances)
        {
            doWithInstance(instance);
        }
    }

    public override void OnGUI()
    {
        var handPoseHelper = GameObject.FindObjectOfType<HandPoseHelper>();

        if (!handPoseHelper) return;

        EditorGUI.BeginChangeCheck();
        ChoiceColorFingers(handPoseHelper);
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(handPoseHelper);
        }
    }

    private void ChoiceColorFingers(HandPoseHelper handPoseHelper)
    {
        if (handPoseHelper.colorFingers.bones == null || handPoseHelper.colorFingers.bones.Count == 0) return;
        if (handPoseHelper.colorFingers.joints == null || handPoseHelper.colorFingers.joints.Count == 0) return;

        _isShowColorFingers = EditorGUILayout.Foldout(_isShowColorFingers, "Color fingers", true);

        if (!_isShowColorFingers) return;

        handPoseHelper.colorFingers.isForEachFinger =
            GUILayout.Toggle(handPoseHelper.colorFingers.isForEachFinger, "For Each Finger");
        if (handPoseHelper.colorFingers.isForEachFinger)
        {
            for (var i = 0; i < handPoseHelper.colorFingers.bones.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Color finger " + (i + 1) + ": ");
                GUILayout.Label(_iconBone, GUILayout.Width(17), GUILayout.Height(17));
                GUILayout.Label("-");
                handPoseHelper.colorFingers.bones[i] =
                    EditorGUILayout.ColorField(new GUIContent(), handPoseHelper.colorFingers.bones[i], false, true,
                        false,
                        GUILayout.Width(40));
                GUILayout.Label("|");
                GUILayout.Label(_iconJoint, GUILayout.Width(17), GUILayout.Height(17));
                GUILayout.Label("-");
                handPoseHelper.colorFingers.joints[i] =
                    EditorGUILayout.ColorField(new GUIContent(), handPoseHelper.colorFingers.joints[i], false, true,
                        false,
                        GUILayout.Width(40));
                GUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Color fingers: ");
            GUILayout.Label(_iconBone, GUILayout.Width(17), GUILayout.Height(17));
            GUILayout.Label("-");
            handPoseHelper.colorFingers.bones[0] =
                EditorGUILayout.ColorField(new GUIContent(), handPoseHelper.colorFingers.bones[0], false, true, false,
                    GUILayout.Width(40));
            GUILayout.Label("|");
            GUILayout.Label(_iconJoint, GUILayout.Width(17), GUILayout.Height(17));
            GUILayout.Label("-");
            handPoseHelper.colorFingers.joints[0] =
                EditorGUILayout.ColorField(new GUIContent(), handPoseHelper.colorFingers.joints[0], false, true, false,
                    GUILayout.Width(40));
            GUILayout.EndHorizontal();
        }
    }
}