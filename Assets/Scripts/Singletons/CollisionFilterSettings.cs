using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

namespace Singletons
{
    public struct CollisionFilterSettings : IComponentData
    {
        public CollisionFilter itemCollisionFilter;
        public CollisionFilter otherCollisionFilter;
    }
}