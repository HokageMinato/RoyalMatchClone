using System.Collections.Generic;
using UnityEngine;

public class GridCellStateHandler : MonoBehaviour
{
    #region PUBLIC_VARIABLES
    [SerializeField] private List<CellLayer> blockers;
    #endregion

    #region PRIVATE_VARIABLES
    private CellLayer blockageLayer
    {
        get
        {
            if (blockers.Count > 0)
                return blockers[blockers.Count - 1];

            return null;
        }
    }
    #endregion

    
    
}
