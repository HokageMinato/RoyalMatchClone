using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBlocker : MonoBehaviour
{

    #region PUBLIC_VARIABLES
    public GameLayer blockLayer;
    public int initial_h;
    public int initial_w;
    #endregion

    #region PUBLIC_REFERENCES
    public Grid grid;
    #endregion

    #region PUBLIC_METHODS
    public virtual void Init(Grid g,int hIndex,int wIndex)
    {
        grid = g;
        initial_h = hIndex;
        initial_w = wIndex;
        OnBlockCells();
    }

    public virtual void Hit(List<GridCell> matchedCells) { }
    public virtual void OnUnblocked() { }
    public virtual void OnBlockCells(){ }
    #endregion

}
