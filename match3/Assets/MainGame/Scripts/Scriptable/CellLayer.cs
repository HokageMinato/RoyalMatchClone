using UnityEngine;


[CreateAssetMenu(fileName = "CellLayer", menuName = "ScriptableObjects/Gameplay/CellLayer")]
public class CellLayer : ScriptableObject
{
    #region PRIVATE_VARIABLES
    [SerializeField] private int layerCount;
    [SerializeField] private int cellUnblockedAtLevel;
    #endregion
    
    #region PUBLIC_VARIABLES
    public bool IsElementBlocked
    {
        get { return layerCount >= cellUnblockedAtLevel; }
    }
    #endregion
}


