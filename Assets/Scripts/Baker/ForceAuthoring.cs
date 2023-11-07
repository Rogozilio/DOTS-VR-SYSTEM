using Components;
using Enums;
using Unity.Entities;
using Unity.Physics.Authoring;
using UnityEngine;

namespace Baker
{
    public class ForceAuthoring : MonoBehaviour
    {
        public DirectionType directionType = DirectionType.Forward;
        public float lenght = 10f;
        [Space]
        public PhysicsCategoryTags belongTo;
        public PhysicsCategoryTags collidesWith;
    }
    
    public class ForceBaker : Baker<ForceAuthoring>
    {
        public override void Bake(ForceAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            Raycast raycast = default;
            raycast.directionType = authoring.directionType;
            raycast.lenght = authoring.lenght;
            raycast.belongTo = authoring.belongTo;
            raycast.collidesWith = authoring.collidesWith;
            AddComponent(entity, raycast);
        }
    }
}