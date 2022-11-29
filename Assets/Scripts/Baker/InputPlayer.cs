using System.Collections;
using System.Collections.Generic;
using Components;
using Unity.Entities;
using UnityEngine;

public class InputPlayer : MonoBehaviour
{
    
}

public class InputPlayerBaker : Baker<InputPlayer>
{
    public override void Bake(InputPlayer authoring)
    {
        AddComponent<InputCamera>();
        AddComponent<InputLeftHand>();
        AddComponent<InputRightHand>();
    }
}
