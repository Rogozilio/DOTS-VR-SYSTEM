using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class ParentAuthoring : MonoBehaviour
{
    public GameObject parent;
}

public class ParentAuthoringBaker : Baker<ParentAuthoring>
{
    public override void Bake(ParentAuthoring authoring)
    {
        var entity = GetEntity(TransformUsageFlags.Dynamic);
        var parent = GetEntity(authoring.parent, TransformUsageFlags.Dynamic);
        AddComponent(entity, new Parent(){ Value = parent});
    }
}