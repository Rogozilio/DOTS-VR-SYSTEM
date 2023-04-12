using Components.Blob;
using Unity.Entities;
using UnityEngine;

namespace Aspects
{
    public readonly partial struct AnimationAssetAspect : IAspect
    {
        private readonly RefRO<AnimationsAsset> _animationAsset;
        
        public ref AnimationItemComponent defaultPose => ref GetPose("default");
        
        public ref AnimationItemComponent GetPose(string name)
        {
            for (var i = 0; i < _animationAsset.ValueRO.asset.Value.animations.Length; i++)
            {
                if (_animationAsset.ValueRO.asset.Value.animations[i].name.ToString() == name)
                    return ref _animationAsset.ValueRO.asset.Value.animations[i];
            }
            Debug.LogError("Pose " + name + " not found. Return default pose");
            return ref _animationAsset.ValueRO.asset.Value.animations[0];
        }
    }
}