using Components.Blob;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


    public class AnimationAuthoring : MonoBehaviour
    {
        public ScriptableObject handAnimationData;
    }

    public class AnimationBaker : Baker<AnimationAuthoring>
    {
        public override void Bake(AnimationAuthoring authoring)
        {
            var builder = new BlobBuilder(Allocator.Temp);
            
            var animationsData = (DefaultListTemplate)authoring.handAnimationData;
            ref var animationsComponent = ref builder.ConstructRoot<AnimationsComponent>();

            var animationItem = builder.Allocate(
                ref animationsComponent.animations,
                animationsData.clips.Count
            );

            for (var i = 0; i < animationsData.clips.Count; i++)
            {
                WriteAnimationItem(ref builder, ref animationItem, animationsData, i);
            }

            var result = builder.CreateBlobAssetReference<AnimationsComponent>(Allocator.Persistent);
            
            builder.Dispose();
            AddBlobAsset(ref result, out var hash);
            AddComponent(new AnimationsAsset()
            {
                asset = result
            });
        }

        private void WriteAnimationItem(ref BlobBuilder builder, ref BlobBuilderArray<AnimationItemComponent> data, DefaultListTemplate animationData, int index)
        {
            //init left hand
            var leftHandAnimBuilder = builder.Allocate(
                ref data[index].leftHand,
                animationData.clips[index].leftHand.joints.Count
            );
            //init right hand
            var rightHandAnimBuilder = builder.Allocate(
                ref data[index].rightHand,
                animationData.clips[index].rightHand.joints.Count
            );
            
            //name
            builder.AllocateString(ref data[index].name,  animationData.clips[index].name);
            //set left hand
            for (var i = 0; i < leftHandAnimBuilder.Length; i++)
            {
                leftHandAnimBuilder[i] = animationData.clips[index].leftHand.joints[i].rotate;
            }
            //set right hand
            for (var i = 0; i < rightHandAnimBuilder.Length; i++)
            {
                rightHandAnimBuilder[i] = animationData.clips[index].rightHand.joints[i].rotate;
            }
        }
    }