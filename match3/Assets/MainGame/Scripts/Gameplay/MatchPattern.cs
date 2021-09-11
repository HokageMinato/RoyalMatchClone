using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MatcherPattern", menuName = "ScriptableObjects/Gameplay/MatcherPattern")]
public class MatchPattern : ScriptableObject
{
    [SerializeField] private List<IndexPair> indexes;
    public string patternName;
     public int Length
    {
        get
        {
            return indexes.Count;
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
