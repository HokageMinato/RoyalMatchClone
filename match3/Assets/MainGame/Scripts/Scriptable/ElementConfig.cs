using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ElementConfig", menuName = "ScriptableObjects/Gameplay/ElementConfig")]
public class ElementConfig : ScriptableObject
{
    //Universal Settings
    public static RenderLayer renderLayer = RenderLayer.ElementLayer;
    public static float SWIPE_ANIM_TIME = 0.5f;
    
    //Instance Specific Settings
    public ElementId elementId;
    
    //PDO Methods
    
}

[System.Serializable]
public class ElementId {
    public string id;

    public bool Equals(ElementId other) {
        return id == other.id;
    }
}
