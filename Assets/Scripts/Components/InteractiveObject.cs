﻿using Enums;
using Unity.Collections;
using Unity.Entities;

namespace Components
{
    public struct InteractiveObject : IComponentData
    {
        public float distanceToHand;
        //interactive options
        public FixedString32Bytes namePose;
        public HandActionType handActionType;
        public InteractiveType interactiveType;
        //smooth option
        public float beginValueSmooth;
        public float valueSmooth;
    }
}