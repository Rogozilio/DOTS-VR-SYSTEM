using System.Collections;
using System.Collections.Generic;
using Scripts;
using UnityEngine;

public class DefaultListTemplate : SaveDataTemplate
{
    public List<HandPoseData> clips;

    public override List<string> GetAllNames
    {
        get
        {
            var names = new List<string>();
            foreach (var value in clips)
            {
                names.Add(value.name);
            }

            return names;
        }
    }

    protected override void OnEnable()
    {
        IsUseDataCollection = true;

        clips ??= new List<HandPoseData>();
    }

    public override void Save(HandPoseData hands, string name)
    {
        hands.name = name;
        this.clips.Clear();
        this.clips.Add(hands);
    }

    public override void SaveElement(HandPoseData hands, string name)
    {
        hands.name = name;
        for (var i = 0; i < this.clips.Count; i++)
        {
            if (this.clips[i].name == hands.name)
            {
                this.clips[i] = hands;
                return;
            }
        }
        
        this.clips.Add(hands);
    }
    
    public override HandPoseData Load(string name)
    {
        foreach (var data in clips)
        {
            if (data.name == name)
            {
                return data;
            }
        }
        
        return new HandPoseData();
    }
}
