using Enums;
using Tags;
using Unity.Entities;
using UnityEngine;

namespace Baker
{
    public class PhysicsHandAuthoring : MonoBehaviour
    {
        public HandType handType;
    }

    public class PhysicsHandAuthoringBaker : Baker<PhysicsHandAuthoring>
    {
        public override void Bake(PhysicsHandAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            
            if(authoring.handType == HandType.Left)
                AddComponent<PhysicsLeftHandTag>(entity);
            else
                AddComponent<PhysicsRightHandTag>(entity);
        }
    }
}