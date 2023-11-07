using Components;
using EnableComponents;
using Unity.Entities;
using UnityEngine;
using Enums;

public class HandAuthoring : MonoBehaviour
{
    public HandType handType;
    public Vector3 offsetRotation;
}

public class HandBaker : Baker<HandAuthoring>
{
    public override void Bake(HandAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        InputHand inputHand = default;
        AddComponent(entity, inputHand);
        Hand hand = default;
        hand.handType = authoring.handType;
        hand.offsetRotation = Quaternion.Euler(authoring.offsetRotation);
        hand.joints.AddReplicate(default, 20);
        AddComponent(entity, hand);
        
        EnableDynamicState enableDynamicState = default;
        EnableStaticState enableStaticState = default;
        AddComponent(entity, enableDynamicState);
        AddComponent(entity, enableStaticState);
        SetComponentEnabled<EnableDynamicState>(entity, false);
        SetComponentEnabled<EnableStaticState>(entity, false);
    }
}
