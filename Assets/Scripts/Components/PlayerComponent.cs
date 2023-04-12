using Enums;
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

        public bool isAlwaysRotate;
        public bool isTurnOnce;
        public quaternion nextRotation;
        public float angularVelocity;
        public float speedRotate;
        public int angleRotate;
        
        //Smooth options
        public DeltaType deltaType;
    }
}