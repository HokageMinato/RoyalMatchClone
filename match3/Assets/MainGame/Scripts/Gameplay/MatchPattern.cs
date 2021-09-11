using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MatcherPattern", menuName = "ScriptableObjects/Gameplay/MatcherPattern")]
public class MatchPattern : ScriptableObject
{
    [SerializeField] private IndexPair[] indexes;
    public string patternName;
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
}

[System.Serializable]
public class IndexPair
{
    public int heightOffset;
    public int widthOffset;


    public int i_Offset
    {
        get
        {
            return heightOffset;
        }
    }
    
    public int j_Offset
    {
        get
        {
            return widthOffset;
        }
    }
}
