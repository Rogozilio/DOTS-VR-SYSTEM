using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using UnityEngine;

public class HeadAuthoring : MonoBehaviour
{
    
}

public class HeadBaker : Baker<HeadAuthoring>
{
    public override void Bake(HeadAuthoring authoring)
    {
        AddComponent<InputCamera>();
    }
}
