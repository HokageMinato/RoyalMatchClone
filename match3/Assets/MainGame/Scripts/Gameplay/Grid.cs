using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridConstants {
    public const int NO_CELL = 0;
}


public class Grid : Singleton<Grid>
{

    #region PRIVATE_VARIABLES
    [SerializeField] private GridCell gridCellPrefab;
    [SerializeField] private GridColoumnCollapser coloumnCollapser;
    [SerializeField] private Transform[] layerTransforms;
    #endregion
    
    #region PUBLIC_VARIABLES
    public GridCell this[int i,int j]
    {
        get { return _grid[i][j]; }
        set { _grid[i][j] = value; }
    }

   
    public int GridHeight
    { get { return GameplayManager.instance.levelData.gridHeight; } }
    
    public int GridWidth
    { get { return GameplayManager.instance.levelData.gridWidth; } }

    
    
    #endregion
   
    #region PRIVATE_VARIABLES
    private GridCell[][] _grid;
  //  private GridDesignTemp _levelData;
    #endregion


    #region UNITY_CALLBACKS
    public void GenerateGrid()
    {
        CreateGrid();
        coloumnCollapser.Init();
    }

    #endregion

    #region PUBLIC_METHODS

    

    public bool AreNeighbours(GridCell firstCell, GridCell secondCell){

        bool isVerticalPositionSame = firstCell.HIndex == secondCell.HIndex;
        bool isHorizontalPositionSame = firstCell.WIndex == secondCell.WIndex;
        int cellDistance;

        if (isHorizontalPositionSame)
        {
            cellDistance = Mathf.Abs(firstCell.HIndex - secondCell.HIndex);
        }
        else if (isVerticalPositionSame)
        {
            cellDistance = Mathf.Abs(firstCell.WIndex - secondCell.WIndex);
        }
        else 
        {
            return false;
        }

        bool areNeighbours = cellDistance <=1  && (isVerticalPositionSame || isHorizontalPositionSame);

        return areNeighbours;
    }


    public Transform GetLayerTransformParent(RenderLayer renderLayer) {
        return layerTransforms[(int)renderLayer];
    }
    #endregion

    #region PRIVATE_METHODS
    
    private void CreateGrid()
    {
        int c = 0;
        GridDesignTemp levelData = GameplayManager.instance.levelData;
        
        _grid = new GridCell[levelData.gridHeight][];
       

        for (int i = 0; i < levelData.gridHeight; i++)
        {
            _grid[i] = new GridCell[levelData.gridWidth];

            for (int j = 0; j < levelData.gridWidth; j++)
            {
                if (levelData.gridDesignTemp[i, j] == GridConstants.NO_CELL)
                    continue;
                 
                CreateCellAt(j, i);
                c++;
            }
            
        }

    }



    private void CreateCellAt(int j, int i)
    {
        GridDesignTemp levelData = GameplayManager.instance.levelData;
        GridCell cell = Instantiate(gridCellPrefab);
        Vector3 newPosition = transform.position + new Vector3(j, -i) * GridDesignTemp.gridSpacing;
        newPosition.x -= levelData.maxWidthMidPointForThisPattern;  // | To make sure gridPivot is in center
        newPosition.y += levelData.maxHeightMidPointForThisPattern;
        
        
        cell.transform.SetParent(GetLayerTransformParent(cell.RenderLayer));
        cell.transform.position = newPosition;
        cell.Init(i,j);
        cell.gameObject.name = $"({i},{j})";
        _grid[i][j] = cell;
    }
    #endregion

    #region PUBLIC_METHODS

    public void CollapseColoumns(MatchExecutionData executionData)
    {
        coloumnCollapser.CollapseColomuns(executionData);
    }

    public void LockDirtyColoumns(MatchExecutionData executionData) {

        List<GridCell> cells = executionData.patternCells;
        cells.Add(executionData.firstCell);
        cells.Add(executionData.secondCell);
        
        for (int i = 0; i < cells.Count; i++)
        {
            int coloumnIndex = cells[i].WIndex;
            if (!executionData.dirtyColoumns.Contains(coloumnIndex))
            {
                LockColoumn(coloumnIndex);
                executionData.dirtyColoumns.Add(coloumnIndex);
            }
        }

        void LockColoumn(int coloumnIndex)
        {
            for (int c = 0; c < GridHeight; c++)
            {
                GridCell cell = this[c, coloumnIndex];
                if (cell)
                    cell.LockCell(executionData);
            }
        }
    }

    

    public void UnlockCells(MatchExecutionData matchExecutionData)
    {
        
        for (int i = 0; i < GridWidth; i++)
        {
            for (int j = 0; j < GridHeight ; j++)
            {
                GridCell cell = _grid[i][j];
                if (cell)
                {
                    if (cell.executionData!=null && cell.executionData.Equals(matchExecutionData))
                    {
                        cell.UnlockCell();
                    }
                }
            }
        }
        
    }

    #endregion

}



