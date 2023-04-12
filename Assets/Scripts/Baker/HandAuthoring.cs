using Components;
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
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
        InputHand inputHand = default;
        AddComponent(entity, inputHand);
        Hand hand = default;
        hand.handType = authoring.handType;
        hand.joints.AddReplicate(default, 20);
        AddComponent(entity, hand);
    }
}
