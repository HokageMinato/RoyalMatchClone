using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "MatcherPattern", menuName = "ScriptableObjects/Gameplay/MatcherPattern")]
public class MatchPattern : ScriptableObject , IEquatable<MatchPattern>
{
    [SerializeField] private IndexPair[] indexes;
     public int Length
    {
        get
        {
            return indexes.Length;
        }
    }

    public IndexPair this[int index]
    {
        get { return indexes[index]; }
    }
    public bool Equals(MatchPattern other)
    {
        return GetHashCode() == other.GetHashCode();
    }
}

[System.Serializable]
public class IndexPair
{
    public int heightOffset;
    public int widthOffset;


    public int I_Offset
    {
        get
        {
            return heightOffset;
        }
    }
    
    public int J_Offset
    {
        get
        {
            return widthOffset;
        }
    }
}
