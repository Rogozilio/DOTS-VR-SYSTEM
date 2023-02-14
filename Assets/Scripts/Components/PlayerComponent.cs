using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct PlayerComponent : IComponentData
    {
        public float3 position;
        public quaternion rotation;
    }
}