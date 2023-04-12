using Enums;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct Hand : IComponentData
    {
        public HandType handType;
        public Entity nearHand;
        public Entity inHand;
        public FixedString32Bytes nextPose;
        public bool isReadyToTake;
        public FixedList512Bytes<quaternion> joints;
        public float3 offsetPosition;
        public quaternion offsetRotation;
    }
}