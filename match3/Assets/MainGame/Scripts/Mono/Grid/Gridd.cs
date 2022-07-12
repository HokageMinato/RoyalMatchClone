﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GridConstants {
    public const int NO_CELL = 0;
}


public class Gridd : Singleton<Gridd>
{

    #region PRIVATE_VARIABLES
    [SerializeField] private GridCell gridCellPrefab;
    [SerializeField] private Transform[] layerTransforms;

    private GridDesignTemp LevelData { get{ return GameplayManager.instance.levelData; } }
    #endregion

    #region PUBLIC_VARIABLES
    public GridCell this[int i, int j]
    {
        get { return _grid[i][j]; }
        set { _grid[i][j] = value; }
    }


    public int GridHeight
    { get { return LevelData.gridHeight; } }

    public int GridWidth
    { get { return LevelData.gridWidth; } }


    public int CellCount {get; private set;}
    #endregion
   
    #region PRIVATE_VARIABLES
    private GridCell[][] _grid;
    #endregion


    #region UNITY_CALLBACKS
    public void GenerateGrid()
    {
        CreateGrid();
        GenerateObstacles();
        InterreferenceGrid();
    }

    

    #endregion


    #region PRIVATE_METHODS


    private void CreateGrid()
    {
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
                  
            }
            
        }
    }

    private void GenerateObstacles()
    {
        GameplayObstacleHandler.instance.Init(LevelData);
    }

    private void InterreferenceGrid()
    {
        CreateBottomReferences();
        CreateBottomLeftReferences();
        CreateBottomRightReferences();
        CreateTopReferences();

        void CreateBottomReferences()
        {
            for (int j = 0; j < GridWidth; j++)
            {
                for (int i = 0; i < GridHeight - 1; i++)
                {

                    if (_grid[i][j] == null ||
                        _grid[i + 1][j] == null || IsCellBlocked(_grid[i + 1][j]))
                        continue;

                    _grid[i][j].bottomCell = _grid[i + 1][j];
                }
            }
        }
        
        void CreateTopReferences()
        {
            for (int j = 0; j < GridWidth; j++)
            {
                for (int i = 0; i < GridHeight - 1; i++)
                {

                    if (_grid[i][j] == null || IsCellBlocked(_grid[i][j]) ||
                        _grid[i + 1][j] == null || IsCellBlocked(_grid[i + 1][j]))
                        continue;

                    _grid[i+1][j].topCell = _grid[i][j];
                }
            }
        }
        void CreateBottomLeftReferences() 
        {
            for (int j = 1; j < GridWidth; j++)
            {
                for (int i = 0; i < GridHeight - 1; i++)
                {

                    GridCell currentCell = _grid[i][j];
                    GridCell bottomLeftCell = _grid[i + 1][j - 1];
                    GridCell bottomCell = _grid[i + 1][j];

                    if (currentCell == null || bottomCell == null ||
                         bottomLeftCell == null || IsCellBlocked(bottomLeftCell))
                        continue;

                    if(IsColoumnBlockedFromHere(bottomLeftCell))
                        currentCell.bottomLeftCell = bottomLeftCell;
                }
            }

        }
        void CreateBottomRightReferences()
        {
            for (int j = 0; j < GridWidth - 1; j++)
            {
                for (int i = 0; i < GridHeight - 1; i++)
                {

                    GridCell currentCell = _grid[i][j];
                    GridCell bottomRightCell = _grid[i + 1][j + 1];
                    GridCell bottomCell = _grid[i + 1][j];

                    if (currentCell == null || bottomCell == null ||
                         bottomRightCell == null || IsCellBlocked(bottomRightCell))
                        continue;

                    if (IsColoumnBlockedFromHere(bottomRightCell))
                        currentCell.bottomRightCell = bottomRightCell;
                }
            }

        }

       

        bool IsColoumnBlockedFromHere(GridCell cell) 
        {
            int h = cell.HIndex;
            int w = cell.WIndex;

            for (int i = h; i >= 0; i--) 
            {
                GridCell ccell = _grid[i][w];
                if (!ccell)
                    continue;

                if (IsCellBlocked(ccell))
                    return true;
            }

            return false;
        }

    }


    private void CreateCellAt(int j, int i)
    {
        GridDesignTemp levelData = GameplayManager.instance.levelData;
        GridCell cell = Instantiate(gridCellPrefab);
        Vector3 newPosition = transform.position + new Vector3(j, -i) * GridDesignTemp.GridSpacing;
        newPosition.x -= levelData.maxWidthMidPointForThisPattern;  // | To make sure gridPivot is in center
        newPosition.y += levelData.maxHeightMidPointForThisPattern;
        
        
        cell.transform.SetParent(GetLayerTransformParent(cell.RenderLayer));
        cell.transform.position = newPosition;
        cell.Init(i,j);
        cell.gameObject.name = $"({i},{j})";
        
        _grid[i][j] = cell;

        CellCount++;
    }

    private bool IsCellBlocked(GridCell gridCell) 
    { 
        return GameplayObstacleHandler.instance.IsCellBlocked(gridCell);
    }

    public bool IsColoumnEmpty(int colIdx) 
    {
        for (int i = 0; i < GridHeight; i++)
        {
            GridCell cell = this[i, colIdx];

            if (cell == null)
                return false;

            if (cell.IsEmpty && !IsCellBlocked(cell))
                return true;
        }
        return false;
    }
    #endregion

    #region PUBLIC_METHODS

    public Transform GetLayerTransformParent(RenderLayer renderLayer)
    {
        return layerTransforms[(int)renderLayer];
    }

    public void LockDirtyColoumns(MatchExecutionData executionData)
    {

        List<GridCell> patternCells = executionData.patternCells;
        //List<GridCell> patternCells = executionData.boosterCells;

        LockColoumn(executionData.firstCell.WIndex,executionData);
        LockColoumn(executionData.secondCell.WIndex,executionData);
        LockColumnOfCells(patternCells,executionData);

    }

    public void LockColumnOfCells(List<GridCell> cells,MatchExecutionData executionData)
    {
        for (int i = 0; i < cells.Count; i++)
        {
            int coloumnIndex = cells[i].WIndex;
            if (!executionData.dirtyColoumns.Contains(coloumnIndex))
            {
                LockColoumn(coloumnIndex,executionData);
                executionData.dirtyColoumns.Add(coloumnIndex);
            }
        }
    }

    void LockColoumn(int coloumnIndex, MatchExecutionData executionData)
    {
        for (int c = 0; c < GridHeight; c++)
        {
            GridCell cell = this[c, coloumnIndex];
            if (cell)
                cell.LockCell(executionData);
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



