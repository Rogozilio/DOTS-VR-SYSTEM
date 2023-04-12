using System.Collections.Generic;
using System.Linq;
using Components.Blob;
using Enums;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;


public class AnimationAuthoring : MonoBehaviour
{
    public HandType handType;
    public List<ScriptableObject> handAnimationData;
}

public class AnimationBaker : Baker<AnimationAuthoring>
{
    private int _indexName;
    public override void Bake(AnimationAuthoring authoring)
    {
        _indexName = 0;
        var builder = new BlobBuilder(Allocator.Temp);

        var listAnimationsData = new List<DefaultListTemplate>();
        listAnimationsData.AddRange(authoring.handAnimationData.OfType<DefaultListTemplate>());
        ref var animationsComponent = ref builder.ConstructRoot<AnimationsComponent>();

        var animationItem = builder.Allocate(
            ref animationsComponent.animations,
            listAnimationsData.Sum(animationData => animationData.clips.Count)
        );
        
        foreach (var animationData in listAnimationsData)
        {
            for (var j = 0; j < animationData.clips.Count; j++)
            {
                WriteAnimationItem(ref builder, ref animationItem, animationData, j, authoring.handType);
            }
        }

        var result = builder.CreateBlobAssetReference<AnimationsComponent>(Allocator.Persistent);

        builder.Dispose();
        AddBlobAsset(ref result, out var hash);
        Entity entity = GetEntity(authoring, TransformUsageFlags.Dynamic);
        AddComponent(entity,new AnimationsAsset()
        {
            asset = result
        });
    }

    private void WriteAnimationItem(ref BlobBuilder builder, ref BlobBuilderArray<AnimationItemComponent> animationItem,
        DefaultListTemplate animationData, int index, HandType handType)
    {
        switch (handType)
        {
            case HandType.Left:
                animationItem[_indexName].offsetPosition = animationData.clips[index].leftHand.attachPosition;
                animationItem[_indexName].offsetRotation = animationData.clips[index].leftHand.attachRotation;
                var leftHandAnimBuilder = builder.Allocate(
                    ref animationItem[_indexName].joints,
                    animationData.clips[index].leftHand.joints.Count
                );
                for (var i = 0; i < leftHandAnimBuilder.Length; i++)
                {
                    leftHandAnimBuilder[i] = animationData.clips[index].leftHand.joints[i].rotate;
                }
                break;
            
            case HandType.Right:
                animationItem[_indexName].offsetPosition = animationData.clips[index].rightHand.attachPosition;
                animationItem[_indexName].offsetRotation = animationData.clips[index].rightHand.attachRotation;
                var rightHandAnimBuilder = builder.Allocate(
                    ref animationItem[_indexName].joints,
                    animationData.clips[index].rightHand.joints.Count);
                for (var i = 0; i < rightHandAnimBuilder.Length; i++)
                {
                    rightHandAnimBuilder[i] = animationData.clips[index].rightHand.joints[i].rotate;
                }
                break;
            
            default:
                Debug.LogError("Hand is not left or right");
                break;
        }
        
        builder.AllocateString(ref animationItem[_indexName++].name, animationData.clips[index].name);
    }
}