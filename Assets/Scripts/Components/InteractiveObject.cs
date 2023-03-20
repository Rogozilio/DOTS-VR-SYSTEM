using Enums;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct InteractiveObject : IComponentData
    {
        public FixedString32Bytes namePose;
        public float distanceToHand;
        public SmoothlyState smoothlyState;
    }
}