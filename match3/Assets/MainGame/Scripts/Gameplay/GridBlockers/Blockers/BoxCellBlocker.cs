using System.Collections.Generic;
using UnityEngine;

public class BoxCellBlocker : CellBlocker
{
    public int requiredHitCount;
    public SpriteRenderer spriteRenderer;
    public Sprite[] spriteLayers;
    GridCell targetCell;

    public override void Init(Grid g)
    {
        base.Init(g);
        OnBlockCells();
    }

    public override void OnBlockCells()
    {
        targetCell = grid[initial_h, initial_w];
        transform.position = targetCell.transform.position;
        targetCell.SetBlocker(this);
        UpdateView();
    }

    public override bool IsNeighbourOf(GridCell otherCell)
    {
        return targetCell.IsNeighbourOf(otherCell);
    }

    public override void Hit(List<GridCell> matchedCells)
    {

        
        for (int i = 0; i < matchedCells.Count; i++)
        {
            if (matchedCells[i].IsNeighbourOf(targetCell)) {
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

        
    }

    public override void OnUnblocked()
    {
        targetCell.SetBlocker(null);
        Destroy(gameObject);
    }


    private void UpdateView()
    {
        spriteRenderer.sortingOrder = (int)blockLayer;
        spriteRenderer.sprite = spriteLayers[requiredHitCount];
    }
}
