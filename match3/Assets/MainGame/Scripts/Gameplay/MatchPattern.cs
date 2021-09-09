using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MatcherPattern", menuName = "ScriptableObjects/Gameplay/MatcherPattern")]
public class MatchPattern : ScriptableObject
{
    [SerializeField] private List<IndexPair> indexes;
    public int iMax;
    public int jMax;

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
    public int widthOffset;
    public int heightOffset;
}
