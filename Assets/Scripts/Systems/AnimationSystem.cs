using Components.Blob;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    [BurstCompile]
    public partial struct AnimationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var anim = SystemAPI.GetSingleton<AnimationsAsset>();
            for (var i = 0; i < anim.asset.Value.animations.Length; i++)
            {
                Debug.Log(anim.asset.Value.animations[i].name);
            }
        }
    }
}