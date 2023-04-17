using Components;
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
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
        InputHand inputHand = default;
        AddComponent(entity, inputHand);
        Hand hand = default;
        hand.handType = authoring.handType;
        hand.offsetRotation = Quaternion.Euler(authoring.offsetRotation);
        hand.joints.AddReplicate(default, 20);
        AddComponent(entity, hand);
    }
}
