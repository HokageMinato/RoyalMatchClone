using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellBlocker : MonoBehaviour
{
    public BlockLayer blockLayer;
    public int initial_h;
    public int initial_w;

    public Grid grid;

    public virtual void Init(Grid g)
    {
        grid = g;
    }

    public virtual bool IsNeighbourOf(GridCell otherCell) { return false; }
    public virtual void Hit(List<GridCell> matchedCells) { }
    public virtual void OnUnblocked() { }

    public virtual void OnBlockCells()
    {

    }



}
