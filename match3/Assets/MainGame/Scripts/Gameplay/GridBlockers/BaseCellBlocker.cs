using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCellBlocker : MonoBehaviour
{

    #region PUBLIC_VARIABLES
    public RenderLayer renderLayer;
    public int initial_h;
    public int initial_w;
    [SerializeField]private GridCell _targetCell;
    #endregion

    #region PUBLIC_REFERENCES
    #endregion

    #region PUBLIC_METHODS
    public virtual void Init(GridCell gridCell,int hIndex,int wIndex)
    {
        initial_h = hIndex;
        initial_w = wIndex;
        _targetCell = gridCell;
        OnBlockCells();
    }

    public virtual void Hit(MatchExecutionData executionData) { }
    public virtual void OnUnblocked() {}
    public virtual void OnBlockCells(){}

    public virtual bool DoesBlockThisCell(GridCell cell) {
        return DoesBlockCell() && _targetCell == cell;
    }

    public virtual bool DoesBlockCell() 
    {
        return renderLayer == RenderLayer.ElementBlockerLayer;
    }

    internal void UpdateRenderLayer()
    {
        transform.SetParent(Grid.instance.GetLayerTransformParent(renderLayer));
    }
    #endregion

}
