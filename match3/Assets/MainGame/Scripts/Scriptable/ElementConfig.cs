using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ElementConfig", menuName = "ScriptableObjects/Gameplay/ElementConfig")]
public class ElementConfig : ScriptableObject
{
    //Universal Settings
    public static RenderLayer renderLayer = RenderLayer.ElementLayer;
    public static float SWIPE_ANIM_TIME = 2f;
    
    //Instance Specific Settings
    public ElementId elementId;
    public AnimationCurve curve;


    public bool Equals(ElementConfig other)
    {
        return other.elementId.id == elementId.id;
    }
}

[System.Serializable]
public class ElementId {
    public string id;

    public bool Equals(ElementId other) {
        return id == other.id;
    }
}
