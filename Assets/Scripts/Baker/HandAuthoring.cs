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
        AddComponent(inputHand);
        Hand hand = default;
        hand.joints.AddReplicate(default, 20);
        AddComponent(hand);
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
