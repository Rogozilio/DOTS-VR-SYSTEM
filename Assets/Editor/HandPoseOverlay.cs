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
using SaveDataTemplate = Scripts.SaveDataTemplate;

[Overlay(typeof(SceneView), "Hand Pose")]
public class HandPoseOverlay : IMGUIOverlay
{
    private static List<HandPoseOverlay> instances = new List<HandPoseOverlay>();

    private bool _isShowColorFingers;
    private bool _isSelectedSaveFile;
    private bool _isNameExist;

    private Texture2D _iconBone;
    private Texture2D _iconJoint;

    private string _nameSaveData;
    private string _namePose;

    private int _popupNamePose;

    private readonly float _widthLabel = 70f;
    private readonly float _widthField = 100f;
    private readonly float _widthButton = 70f;

    private HandPoseHelper _handPoseHelper;
    private SaveDataTemplate _template;

    private bool _isUseDataCollection => _template && _template.GetIsUseDataCollection;
    private string _pathAsset => _handPoseHelper.saveDataSetting.path + "/" + _nameSaveData + ".asset";

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
        _handPoseHelper = GameObject.FindObjectOfType<HandPoseHelper>();

        if (!_handPoseHelper) return;

        SaveData(_handPoseHelper);

        EditorGUI.BeginChangeCheck();
        ChoiceColorFingers(_handPoseHelper);
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(_handPoseHelper);
        }
    }

    private void SaveData(HandPoseHelper handPoseHelper)
    {
        if (!_template) CreateSO(handPoseHelper);

        if (Selection.activeObject?.GetType().BaseType == typeof(SaveDataTemplate))
        {
            _isSelectedSaveFile = true;
            _nameSaveData = Selection.activeObject?.name;
            FindPoseByName();
        }
        else if (_isSelectedSaveFile)
        {
            _isSelectedSaveFile = false;
            _nameSaveData = string.Empty;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Template: ", GUILayout.Width(_widthLabel));
        EditorGUI.BeginChangeCheck();
        handPoseHelper.popupIndexTemplate = EditorGUILayout.Popup(handPoseHelper.popupIndexTemplate,
            handPoseHelper.saveDataSetting.names.ToArray()
            , GUILayout.Width(_widthField));
        if (EditorGUI.EndChangeCheck())
        {
            CreateSO(handPoseHelper);
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Name file:", GUILayout.Width(_widthLabel));
        GUI.enabled = !_isSelectedSaveFile;
        _nameSaveData = EditorGUILayout.TextField(_nameSaveData, GUILayout.Width(_widthField));
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();

        if (_isUseDataCollection)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Name pose:", GUILayout.Width(_widthLabel));
            if (_isSelectedSaveFile)
            {
                var template = AssetDatabase.LoadAssetAtPath<SaveDataTemplate>(_pathAsset);
                _popupNamePose = EditorGUILayout.Popup(_popupNamePose, template.GetAllNames.ToArray(),
                    GUILayout.Width(_widthField));
                _namePose = template.GetAllNames[_popupNamePose];
            }
            else
                _namePose = EditorGUILayout.TextField(_namePose, GUILayout.Width(_widthField));

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.BeginHorizontal();
        ShowButtonLoad();

        if (_isSelectedSaveFile) ShowButtonOverwrite();
        else ShowButtonCreate();

        if (_isNameExist) ShowButtonReplace();
        else ShowButtonAdd();
        EditorGUILayout.EndHorizontal();
    }

    private void CreateSO(HandPoseHelper handPoseHelper)
    {
        var type = handPoseHelper.saveDataSetting.templates[handPoseHelper.popupIndexTemplate].GetClass();
        _template = (SaveDataTemplate)ScriptableObject.CreateInstance(type);
    }

    private void FindPoseByName()
    {
        if (!_isUseDataCollection) return;

        var template = AssetDatabase.LoadAssetAtPath<SaveDataTemplate>(_pathAsset);
        _isNameExist = template.FindByName(_namePose, out HandPoseData handPoseData);
    }

    private void ShowButtonLoad()
    {
        GUI.enabled = _isSelectedSaveFile && (!_isUseDataCollection || (_isUseDataCollection && _isNameExist));

        if (GUILayout.Button("Load", GUILayout.Width(_widthButton)))
        {
            var template = AssetDatabase.LoadAssetAtPath<SaveDataTemplate>(_pathAsset);
            template.FindByName(_namePose, out HandPoseData handPoseData);
            _handPoseHelper.SetHandPoseData(_isNameExist ? handPoseData : template.Load());
        }

        GUI.enabled = true;
    }

    private void ShowButtonCreate()
    {
        if (GUILayout.Button("Create", GUILayout.Width(_widthButton)))
        {
            if (_handPoseHelper.saveDataSetting.path == string.Empty)
            {
                Debug.LogError("Save data path empty. Set path in prefab HandPoseHelper in setting save data.");
                return;
            }

            var type = _handPoseHelper.saveDataSetting.templates[_handPoseHelper.popupIndexTemplate].GetClass();
            if (type.BaseType != typeof(SaveDataTemplate))
            {
                Debug.LogError("Script " + type + " must inherit from SaveDataTemplate.");
                return;
            }

            _template.Save(_handPoseHelper.GetHandPoseData(), _nameSaveData);
            AssetDatabase.CreateAsset(_template, _pathAsset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            _template = null;
        }
    }

    private void ShowButtonOverwrite()
    {
        if (GUILayout.Button("Overwrite", GUILayout.Width(_widthButton)))
        {
            var template = AssetDatabase.LoadAssetAtPath<SaveDataTemplate>(_pathAsset);
            template.Save(_handPoseHelper.GetHandPoseData(), _isNameExist ? _namePose : _nameSaveData);
        }
    }

    private void ShowButtonAdd()
    {
        GUI.enabled = _isSelectedSaveFile && _isUseDataCollection;
        if (GUILayout.Button("Add", GUILayout.Width(_widthButton)))
        {
            var template = AssetDatabase.LoadAssetAtPath<SaveDataTemplate>(_pathAsset);
            template.SaveElement(_handPoseHelper.GetHandPoseData(), _namePose);
        }

        GUI.enabled = true;
    }

    private void ShowButtonReplace()
    {
        GUI.enabled = _isSelectedSaveFile && _isUseDataCollection;
        if (GUILayout.Button("Replace", GUILayout.Width(_widthButton)))
        {
            var template = AssetDatabase.LoadAssetAtPath<SaveDataTemplate>(_pathAsset);
            template.SaveElement(_handPoseHelper.GetHandPoseData(), _namePose);
        }

        GUI.enabled = true;
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