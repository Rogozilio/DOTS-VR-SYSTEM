using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.Toolbars;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    [Overlay(typeof(SceneView), "Hand Pose Toolbar")]
    public class HandPoseToolbarOverlay : ToolbarOverlay
    {
        public HandPoseToolbarOverlay() : base(HandPoseButton.Id)
        {
        }

        [EditorToolbarElement(Id, typeof(SceneView))]
        class HandPoseButton : EditorToolbarToggle
        {
            public const string Id = "HandPoseToolbar/button";

            private GameObject _handPoseHelper;

            public HandPoseButton()
            {
                _handPoseHelper = GameObject.FindObjectOfType<Scripts.HandPoseHelper>()?.gameObject;

                icon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/HandPoseHelper/Editor/Sprites/hand.png");
                tooltip = "Enable/Disable Hand Pose Mode";
                value = _handPoseHelper;

                this.RegisterValueChangedCallback(ToggleSwitch);
            }

            private void ToggleSwitch(ChangeEvent<bool> evt)
            {
                if (evt.newValue)
                {
                    if (_handPoseHelper) return;

                    var root = Resources.Load<GameObject>("HandPoseHelper");
                    _handPoseHelper =
                        (GameObject)PrefabUtility.InstantiatePrefab(root);
                }
                else
                {
                    PrefabUtility.ApplyObjectOverride(_handPoseHelper.GetComponent<Scripts.HandPoseHelper>(),
                        PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(_handPoseHelper), InteractionMode.AutomatedAction);
                    if (_handPoseHelper)
                        GameObject.DestroyImmediate(_handPoseHelper);
                }
            }
        }
    }
}