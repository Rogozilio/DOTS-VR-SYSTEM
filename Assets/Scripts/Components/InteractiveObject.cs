using Enums;
using Unity.Collections;
using Unity.Entities;

namespace Components
{
    public struct InteractiveObject : IComponentData
    {
        public InHandType inHand;
        public float distanceToHand;
        //interactive options
        public FixedString32Bytes namePose;
        public bool isSwitchHand;
        //smooth option
        public float beginValueSmooth;
        public float valueSmooth;
    }
}