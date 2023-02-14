using System;
using System.Collections;
using System.Collections.Generic;
using Components;
using Tags;
using Unity.Entities;
using UnityEngine;
using Enums;

public class HandAuthoring : MonoBehaviour
{
    public HandType handType;
}

public class HandBaker : Baker<HandAuthoring>
{
    public override void Bake(HandAuthoring authoring)
    {
        InputHand inputHand = default;
        inputHand.offsetRotation = authoring.transform.rotation;
        AddComponent(inputHand);
        AddComponent<Hand>();
        switch (authoring.handType)
        {
            case HandType.Left:
                AddComponent<LeftHandTag>();
                break;
            case HandType.Right:
                AddComponent<RightHandTag>();
                break;
        }
    }
}
