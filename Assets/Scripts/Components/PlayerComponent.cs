using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct PlayerComponent : IComponentData
    {
        public float3 nextPosition;
        public float3 velocity;
        public float speed;
    }
}