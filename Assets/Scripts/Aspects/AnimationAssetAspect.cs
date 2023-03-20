using Components.Blob;
using Unity.Entities;
using UnityEngine;

namespace Aspects
{
    public readonly partial struct AnimationAssetAspect : IAspect
    {
        private readonly RefRO<AnimationsAsset> animationAsset;
        
        public ref AnimationItemComponent defaultPose => ref GetPose("default");
        
        public ref AnimationItemComponent GetPose(string name)
        {
            for (var i = 0; i < animationAsset.ValueRO.asset.Value.animations.Length; i++)
            {
                if (animationAsset.ValueRO.asset.Value.animations[i].name.ToString() == name)
                    return ref animationAsset.ValueRO.asset.Value.animations[i];
            }
            Debug.LogError("Pose " + name + " not found. Return default pose");
            return ref animationAsset.ValueRO.asset.Value.animations[0];
        }
    }
}