using Components;
using Unity.Entities;
using UnityEngine;

namespace Baker
{
    public class CameraAuthoring : MonoBehaviour
    {
        
    }

    public class CameraAuthoringBaker : Baker<CameraAuthoring>
    {
        public override void Bake(CameraAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent<InputCamera>(entity);
        }
    }
}