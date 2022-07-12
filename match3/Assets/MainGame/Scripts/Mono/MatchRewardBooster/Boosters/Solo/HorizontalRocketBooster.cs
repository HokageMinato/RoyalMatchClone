using System;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalRocketBooster : MonoBehaviour, IMatchGameplayRewardBooster
{
    private MatchExecutionData _executionData;
    private Gridd _grid;
    private List<GridCell> _cellsToBeHit = new List<GridCell>();


    public void UseBooster(MatchExecutionData executionData, Gridd grid)
    {
        Debug.Log("From normal Rocket Booster");
        _executionData = executionData;
        _grid = grid;
        
        SetSelfAtPlaceboPosition();
        EmptyFakeBoosterElement();
        FindCellsToHit();
        _grid.LockColumnOfCells(_cellsToBeHit, executionData);
        ExtractElementsFromCellList();
    }

    

    private void SetSelfAtPlaceboPosition()
    {
        GridCell targetCell = _executionData.boosterCell;
        transform.position = targetCell.transform.position;
        
    }

    private void FindCellsToHit()
    {
        int rowIndex = _executionData.boosterCell.HIndex;
        int gridWidth = _grid.GridWidth;

        for (int i = 0; i < gridWidth; i++)
        {
            GridCell cell = _grid[rowIndex, i];
            if (cell != null && !cell.IsEmpty && !GameplayObstacleHandler.instance.IsCellBlocked(cell))
            {
                _cellsToBeHit.Add(cell);
            }
        }
        
    }

    private void EmptyFakeBoosterElement()
    {
       Element element =  _executionData.boosterCell.GetElement();
       element.DestroyElement(); //empty the booster cell
    }

    private void ExtractElementsFromCellList()
    {
        GameplayBoosterHitData boosterHitData = _executionData.boosterHitData;
       
        for (int i = 0; i < _cellsToBeHit.Count; i++)
        {
            GridCell item = _cellsToBeHit[i];
            boosterHitData.AddElement(item.GetElement());
        }
        
    }

    
}
