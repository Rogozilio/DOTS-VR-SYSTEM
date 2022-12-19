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

        SaveData();

        EditorGUI.BeginChangeCheck();
        ChoiceColorFingers();
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(_handPoseHelper);
        }
    }

    private void SaveData()
    {
        if (!_template) CreateSO(_handPoseHelper);

        if (Selection.activeObject?.GetType().BaseType == typeof(SaveDataTemplate))
        {
            _isSelectedSaveFile = true;
            _nameSaveData = Selection.activeObject?.name;
            FindTemplateByName(Selection.activeObject?.GetType().ToString());
            FindPoseByName();
        }
        else if (_isSelectedSaveFile)
        {
            _isSelectedSaveFile = false;
            _nameSaveData = string.Empty;
            _namePose = string.Empty;
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Template: ", GUILayout.Width(_widthLabel));
        GUI.enabled = !_isSelectedSaveFile;
        EditorGUI.BeginChangeCheck();
        _handPoseHelper.popupIndexTemplate = EditorGUILayout.Popup(_handPoseHelper.popupIndexTemplate,
            _handPoseHelper.saveDataSetting.names.ToArray()
            , GUILayout.Width(_widthField));
        if (EditorGUI.EndChangeCheck())
        {
            CreateSO(_handPoseHelper);
        }
        GUI.enabled = true;

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
            var template = AssetDatabase.LoadAssetAtPath<SaveDataTemplate>(_pathAsset);
            _namePose = EditorGUILayout.TextField(_namePose, GUILayout.Width(_widthField));
            if (_isSelectedSaveFile && template.GetAllNames != null && template.GetAllNames.Count > 0)
            {
                var lastRect = GUILayoutUtility.GetLastRect();
                var newRect = new Rect(new Vector2(lastRect.x + lastRect.size.x, lastRect.y), new Vector2(20, 20));
                _namePose = DrawDropdown(newRect, GUIContent.none, template.GetAllNames);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        ShowButtonLoad();

        if (_isSelectedSaveFile) ShowButtonOverwrite();
        else ShowButtonCreate();

        if (_isNameExist) ShowButtonReplace();
        else ShowButtonAdd();
        EditorGUILayout.EndHorizontal();
    }

    private string DrawDropdown(Rect position, GUIContent label, List<string> allNames)
    {
        if (!EditorGUI.DropdownButton(position, label, FocusType.Passive))
        {
            return _namePose;
        }

        void handleItemClicked(object parameter)
        {
            _namePose = parameter.ToString();
        }

        GUI.FocusControl(null);
        GenericMenu menu = new GenericMenu();
        foreach (var name in allNames)
        {
            menu.AddItem(new GUIContent(name), false, handleItemClicked, name);
        }

        menu.DropDown(position);

        return _namePose;
    }

    private void CreateSO(HandPoseHelper handPoseHelper)
    {
        var type = handPoseHelper.saveDataSetting.templates[handPoseHelper.popupIndexTemplate].GetClass();
        _template = (SaveDataTemplate)ScriptableObject.CreateInstance(type);
    }

    private void FindTemplateByName(string nowName)
    {
        for(var i = 0;i < _handPoseHelper.saveDataSetting.templates.Count;i++)
        {
            if (nowName == _handPoseHelper.saveDataSetting.templates[i].name)
            {
                _handPoseHelper.popupIndexTemplate = i;
                CreateSO(_handPoseHelper);
                return;
            }
        }
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
            if (_isUseDataCollection)
                template.Save(_handPoseHelper.GetHandPoseData(), _namePose != string.Empty ? _namePose : _nameSaveData);
            else 
                template.Save(_handPoseHelper.GetHandPoseData(), _isNameExist ? _namePose : _nameSaveData);

            EditorUtility.SetDirty(template);
        }
    }

    private void ShowButtonAdd()
    {
        GUI.enabled = _isSelectedSaveFile && _isUseDataCollection && _namePose != string.Empty;
        if (GUILayout.Button("Add", GUILayout.Width(_widthButton)))
        {
            var template = AssetDatabase.LoadAssetAtPath<SaveDataTemplate>(_pathAsset);
            template.SaveElement(_handPoseHelper.GetHandPoseData(), _namePose);
            EditorUtility.SetDirty(template);
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
            EditorUtility.SetDirty(template);
        }

        GUI.enabled = true;
    }

    private void ChoiceColorFingers()
    {
        if (_handPoseHelper.colorFingers.bones == null || _handPoseHelper.colorFingers.bones.Count == 0) return;
        if (_handPoseHelper.colorFingers.joints == null || _handPoseHelper.colorFingers.joints.Count == 0) return;

        _isShowColorFingers = EditorGUILayout.Foldout(_isShowColorFingers, "Color fingers", true);

        if (!_isShowColorFingers) return;

        _handPoseHelper.colorFingers.isForEachFinger =
            GUILayout.Toggle(_handPoseHelper.colorFingers.isForEachFinger, "For Each Finger");
        if (_handPoseHelper.colorFingers.isForEachFinger)
        {
            for (var i = 0; i < _handPoseHelper.colorFingers.bones.Count; i++)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("Color finger " + (i + 1) + ": ");
                GUILayout.Label(_iconBone, GUILayout.Width(17), GUILayout.Height(17));
                GUILayout.Label("-");
                _handPoseHelper.colorFingers.bones[i] =
                    EditorGUILayout.ColorField(new GUIContent(), _handPoseHelper.colorFingers.bones[i], false, true,
                        false,
                        GUILayout.Width(40));
                GUILayout.Label("|");
                GUILayout.Label(_iconJoint, GUILayout.Width(17), GUILayout.Height(17));
                GUILayout.Label("-");
                _handPoseHelper.colorFingers.joints[i] =
                    EditorGUILayout.ColorField(new GUIContent(), _handPoseHelper.colorFingers.joints[i], false, true,
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
            _handPoseHelper.colorFingers.bones[0] =
                EditorGUILayout.ColorField(new GUIContent(), _handPoseHelper.colorFingers.bones[0], false, true, false,
                    GUILayout.Width(40));
            GUILayout.Label("|");
            GUILayout.Label(_iconJoint, GUILayout.Width(17), GUILayout.Height(17));
            GUILayout.Label("-");
            _handPoseHelper.colorFingers.joints[0] =
                EditorGUILayout.ColorField(new GUIContent(), _handPoseHelper.colorFingers.joints[0], false, true, false,
                    GUILayout.Width(40));
            GUILayout.EndHorizontal();
        }
    }
}