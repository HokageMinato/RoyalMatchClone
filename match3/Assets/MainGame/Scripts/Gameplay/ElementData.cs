using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementData", menuName = "ScriptableObjects/Gameplay/ElementData")]
public class ElementData : ScriptableObject
{
    
    public AnimationCurve curve;
    public float swipeAnimationTime;
    public float MatcherWaitRate  {
        get
        {
            return 1f / swipeAnimationTime;
        }
    }


    
}
