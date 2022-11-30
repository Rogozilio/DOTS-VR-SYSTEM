using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct InputHand : IComponentData
    {
        public float3 position;
        public quaternion rotation;
        public int trackingState; //Unknown, Tracking, Unavailable
        public float grip;
        public float gripValue;
        public float trigger;
        public float triggerValue;
        //public object hapticDevice;
    }
}