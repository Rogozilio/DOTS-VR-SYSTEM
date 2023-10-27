using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;

namespace Components.Blob
{
    public struct AnimationItemComponent : IComponentData
    {
        public BlobString name;
        public BlobArray<quaternion> joints;
        public float3 offsetPosition;
        public quaternion offsetRotation;
    }
    public struct AnimationsComponent : IComponentData
    {
        public BlobArray<AnimationItemComponent> animations;
    }

    public struct AnimationsAsset : IComponentData
    {
        public BlobAssetReference<AnimationsComponent> asset;
    }
}