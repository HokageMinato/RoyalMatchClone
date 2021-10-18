using System.Collections.Generic;
using UnityEngine;

public class BoxCellBlocker : CellBlocker
{
    #region PUBLIC_VARIABLES
    public int requiredHitCount;
    public SpriteRenderer spriteRenderer;
    public Sprite[] spriteLayers;
    #endregion

    #region PUBLIC_REFERENCES
    GridCell targetCell;
    #endregion

    #region PUBLIC_METHODS
    
    public override void OnBlockCells()
    {
        targetCell = grid[initial_h, initial_w];
        transform.position = targetCell.transform.position;
        targetCell.SetBlocker(this);
        UpdateView();
    }

    
    public override void Hit(List<GridCell> matchedCells)
    {

        for (int i = 0; i < matchedCells.Count; i++)
        {
            GridCell matchedCell = matchedCells[i];
           
            if(grid.AreNeighbours(matchedCell,targetCell))
                requiredHitCount --;
                if (requiredHitCount >= 0)
                {
                    UpdateView();
                }
                else
                {
                    OnUnblocked();
                    break;
                }
            }
        }

    public override void OnUnblocked()
    {
        targetCell.SetBlocker(null);
        GameplayObstacleHandler.instance.DiscardObstacle(this);
    }

    #endregion

    private void UpdateView()
    {
        spriteRenderer.sortingOrder = (int)blockLayer;
        spriteRenderer.sprite = spriteLayers[requiredHitCount];
    }
}
