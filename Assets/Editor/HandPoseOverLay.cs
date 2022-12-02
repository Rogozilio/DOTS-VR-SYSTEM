using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

[Overlay(typeof(SceneView), "Hand Pose")]
public class HandPoseOverLay : IMGUIOverlay
{
    private static List<HandPoseOverLay> instances = new List<HandPoseOverLay>();

    public override void OnCreated()
    {
        instances.Add(this);
    }

    public override void OnWillBeDestroyed()
    {
        instances.Remove(this);
    }

    public static void DoWithInstances(Action<HandPoseOverLay> doWithInstance)
    {
        foreach (var instance in instances)
        {
            doWithInstance(instance);
        }
    }

    public override void OnGUI()
    {
       
    }
}
