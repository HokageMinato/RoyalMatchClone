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
    [SerializeField] private GridColoumn GridColoumnPrefab;
    [SerializeField] private GridCell gridCellPrefab;
   
    #endregion
    
    #region PUBLIC_VARIABLES
    public GridCell this[int i,int j]
    {
        get { return _grid[i,j]; }
        set { _grid[i,j] = value; }
    }

    public GridColoumn this[int j]
    {
        get { return _gridC[j]; }
    }

    public int GridHeight
    { get { return _levelData.gridHeight; } }
    
    public int GridWidth
    { get { return _levelData.gridWidth; } }
   
    #endregion
   
    #region PRIVATE_VARIABLES
    private GridCell[,] _grid;
    private List<GridColoumn> _gridC= new List<GridColoumn>();
    private GridDesignTemp _levelData;
    #endregion


    #region UNITY_CALLBACKS
    public void GenerateGrid()
    {
        _levelData = GameplayManager.instance.levelData;
        CreateGrid();
        SetColoumnCells();
        ToggleColoumnLock(true);
        WaitForGridAnimation(() => { ToggleColoumnLock(false);});      
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
    
    #endregion

    #region PRIVATE_METHODS
    
    private void CreateGrid()
    {
        int c = 0;
        
        _grid = new GridCell[_levelData.gridHeight,_levelData.gridWidth];
       

        for (int i = 0; i < _levelData.gridHeight; i++)
        {
            _gridC.Add(Instantiate(GridColoumnPrefab,transform));

            for (int j = 0; j < _levelData.gridWidth; j++)
            {
                if (_levelData.gridDesignTemp[i, j] == GridConstants.NO_CELL)
                    continue;
                 
                CreateCellAt(j, i);
                c++;
            }

            if (_levelData.gridDesignTemp[0, i] != GridConstants.NO_CELL)
                _gridC[_gridC.Count - 1].transform.position = _grid[0, i].transform.position + Vector3.up * 0.5f;

        }

    }

   

    private void SetColoumnCells()
    {
         for (int i = 0; i < _levelData.gridWidth; i++)
         {
            for (int j = 0; j < _levelData.gridHeight; j++)
            {
                if (_levelData.gridDesignTemp[i, j] == GridConstants.NO_CELL)
                    continue;
        
                _gridC[j].AddCell(_grid[i,j]);
            }
         }
    }

    private void CreateCellAt(int j, int i)
    {
        GridCell cell = Instantiate(gridCellPrefab, transform);
        Vector3 newPosition = transform.position + new Vector3(j, -i) * _levelData.gridSpacing;
        newPosition.x -= _levelData.maxWidthMidPointForThisPattern;  // | To make sure gridPivot is in center
        newPosition.y += _levelData.maxHeightMidPointForThisPattern;//  |
        cell.transform.position = newPosition;
        cell.Init(i,j);
        cell.gameObject.name = $"({i},{j})";
        _grid[i, j] = cell;
        i++;
    }
    



    private void ToggleColoumnLock(bool toggleValue)
    {
        if(toggleValue)
            for (int i = 0; i < _gridC.Count; i++)
                _gridC[i].LockColoumn(null);
        else
            for (int i = 0; i < _gridC.Count; i++)
                _gridC[i].UnLockColoumn();
    }
    
    #endregion

    #region PUBLIC_METHODS

    public void CollapseColoumns(MatchExecutionData executionData)
    {
        for (int l = 0; l < _gridC.Count; l++)
        {
            _gridC[l].CollapseColoumn(executionData);
        }
        
    }

    public void LockDirtyColoumns(MatchExecutionData executionData)
    {
        List<GridCell> cells = executionData.patternCells;
        _gridC[executionData.firstCell.WIndex].LockColoumn(executionData);
        _gridC[executionData.secondCell.WIndex].LockColoumn(executionData);
        for (int i = 0; i < cells.Count; i++)
        {
            _gridC[cells[i].WIndex].LockColoumn(executionData);
            
        }
        
    }

    public void UnlockCells(MatchExecutionData matchExecutionData)
    {
        
        for (int i = 0; i < _levelData.gridWidth; i++)
        {
            for (int j = 0; j < _levelData.gridHeight ; j++)
            {
                GridCell cell = _grid[i, j];
                if (cell)
                {
                    if (cell.executionData!=null && cell.executionData.Equals(matchExecutionData))
                    {
                        _gridC[j].UnLockColoumn();
                    }
                }
            }
        }
        
    }

    #endregion


    #region ASYNC_METHODS
    private void WaitForGridAnimation(Action action)
    {
        StartCoroutine(WaitForGridAnimationRoutine(action));
    }

    private IEnumerator WaitForGridAnimationRoutine(Action action=null)
    {
        yield return new WaitForSeconds(0.2f);
        action?.Invoke();
    }
    #endregion
}



