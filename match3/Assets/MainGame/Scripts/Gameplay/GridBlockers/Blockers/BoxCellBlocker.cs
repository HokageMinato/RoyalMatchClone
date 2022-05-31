using System.Collections.Generic;
using UnityEngine;

public class BoxCellBlocker : BaseCellBlocker
{
    #region PUBLIC_VARIABLES
    public int RequiredHitCount {
        get {
            return spriteLayers.Length - 1;
        }
    }
    public SpriteRenderer spriteRenderer;
    public Sprite[] spriteLayers;
    public int hitCount=0;
    public bool immune;
    public MatchPattern boxBlockerPattern;
    #endregion

    #region PUBLIC_REFERENCES
    GridCell targetCell;
    #endregion



    #region PUBLIC_METHODS
    public override void Init(GridCell gridCell, int hIndex, int wIndex)
    {
        targetCell = gridCell;
        base.Init(gridCell, hIndex, wIndex);
    }

    public override void OnBlockCells()
    {
        transform.position = targetCell.transform.position;
        UpdateView();
    }


    public override void Hit(MatchExecutionData executionData)
    {
        if (immune)
            return;

        List<GridCell> toBeDestoryedCells = executionData.patternCells;
        List<GridCell> edgeCells = new List<GridCell>();
        PatternCompareUtility.GetAllPatternCellsNonAlloc(edgeCells, boxBlockerPattern, initial_h, initial_w);


        for (int i = 0; i < edgeCells.Count; i++)
        {
            if (toBeDestoryedCells.Contains(edgeCells[i])) 
            {
                hitCount++;
                Debug.Log(hitCount);
                if (hitCount < RequiredHitCount)
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
        //targetCell.SetBlocker(null);
        GameplayObstacleHandler.instance.DiscardObstacle(this,targetCell);
    }

    #endregion

    private void UpdateView()
    {
        UpdateRenderLayer();
        spriteRenderer.sprite = spriteLayers[RequiredHitCount-hitCount];
    }
}
