using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

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
        public float2 move;
        public float2 turn;
        public bool isTurnOnce;
    }
}