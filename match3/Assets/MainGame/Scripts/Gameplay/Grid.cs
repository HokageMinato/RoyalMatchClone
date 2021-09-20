using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Grid : Singleton<Grid>
{

    #region PUBLIC_VARIABLES
    public GridColoumn GridColoumnPrefab;
    public GridCell gridCellPrefab;
    public ElementGenerator elementGeneratorPrefab;
    
    
    public GridCell this[int i,int j]
    {
        get { return _grid[i,j]; }
        set { _grid[i,j] = value; }
    }
    public int GridHeight
    {
        get { return GridDesignTemp.gridHeight; }
    }
    
    public int GridWidth
    {
        get { return GridDesignTemp.gridHeight; }
    }
    public bool IsAnimating=false;
    #endregion
   
    #region PRIVATE_VARIABLES
    private GridCell[,] _grid;
    private List<GridColoumn> _gridC= new List<GridColoumn>(); 
    #endregion

    
    #region UNITY_CALLBACKS
    public override void Awake()
    {
        base.Awake();
        CreateGrid();
        CreateElementGenerators();
        SetColoumnCells();
        ToggleColoumnLock(true);
        WaitForGridAnimation(() => { ToggleColoumnLock(false);});
    }
    #endregion

    #region PUBLIC_METHODS

    
    #endregion

    #region PRIVATE_METHODS
    
    private void CreateGrid()
    {
        int c = 0;
        _grid = new GridCell[GridDesignTemp.gridHeight,GridDesignTemp.gridWidth];
       

        for (int i = 0; i < GridDesignTemp.gridHeight; i++)
        {
            _gridC.Add(Instantiate(GridColoumnPrefab,transform));
            for (int j = 0; j < GridDesignTemp.gridWidth; j++)
            {
                if (GridDesignTemp.gridDesignTemp[i, j] == 0)
                    continue;
                 
                CreateCellAt(j, i);
                FillInitialElementAt(j, i,c);
                c++;
            }
        }
        
    }

    private void CreateElementGenerators()
    {
        int i = 0;
        for (int j = 0; j < GridDesignTemp.gridWidth; j++)
        {
            ElementGenerator generator = Instantiate(elementGeneratorPrefab);
            GridCell cell = _grid[i, j];
            
            Vector3 position = cell.transform.position;
            position.y += 2f;
            generator.transform.position = position;
            _gridC[j].SetGenerator(generator);
        }
    }

    private void SetColoumnCells()
    {
         for (int i = 0; i < GridDesignTemp.gridWidth; i++)
         {
            for (int j = 0; j < GridDesignTemp.gridHeight/*-1*/; j++)
            {
                if (GridDesignTemp.gridDesignTemp[i, j] == 0)// || GridDesignTemp.gridDesignTemp[j+1,i] == 0)
                    continue;
        
                _gridC[j].AddCell(_grid[i,j]);
            }
         }
    }

    private void CreateCellAt(int j, int i)
    {
        GridCell cell = Instantiate(gridCellPrefab, transform);
        Vector3 newPosition = transform.position + new Vector3(j, -i) * GridDesignTemp.gridSpacing;
        newPosition.x -= GridDesignTemp.maxWidthMidPointForThisPattern;  // | To make sure gridPivot is in center
        newPosition.y += GridDesignTemp.maxHeightMidPointForThisPattern;//  |
        cell.transform.position = newPosition;
        cell.Init(i,j);
        cell.gameObject.name = $"({i},{j})";
        _grid[i, j] = cell;
        i++;
    
       
    }

    private void FillInitialElementAt(int j, int i,int c)
    {
        GridCell parentCell = _grid[i, j];
        
        if(!parentCell.IsEmpty)
            return;
        
        Element element = elementGeneratorPrefab.GetRandomElement(parentCell);
        Transform elementTransform = element.transform;
        elementTransform.localPosition = new Vector3(0f,15f,0f);
        element.gameObject.name = c.ToString();
        parentCell.SetElement(element);
    }

    private void ToggleColoumnLock(bool toggleValue)
    {
        if(toggleValue)
            for (int i = 0; i < _gridC.Count; i++)
                _gridC[i].LockColoumn();
        else
            for (int i = 0; i < _gridC.Count; i++)
                _gridC[i].UnLockColoumn();
    }
    
    #endregion

  
    public void CollapseColoumns()
    {
        for (int l = 0; l < _gridC.Count; l++)
        {
            _gridC[l].CollapseColoumn();
        }
        
    }

    public void LockDirtyColoumns(MatchExecutionData executionData)
    {
        List<GridCell> cells = executionData.patternCells;
        for (int i = 0; i < cells.Count; i++)
        {
            _gridC[cells[i].HIndex].LockColoumn();    
        }
        
    }

    public void UnlockCells(MatchExecutionData matchExecutionData)
    {
        for (int i = 0; i < GridDesignTemp.gridWidth; i++)
        {
            for (int j = 0; j < GridDesignTemp.gridHeight ; j++)
            {
                GridCell cell = _grid[i, j];
                if (cell)
                {
                    if (cell.executionData.Equals(matchExecutionData))
                    {
                        cell.SetExecutionData(MatchExecutionData.GetDefaultExecutionData());
                        _gridC[j].UnLockColoumn();
                    }
                }
            }
        }
    }

    private void WaitForGridAnimation(Action action)
    {
        StartCoroutine(WaitForGridAnimationRoutine(action));
    }

    private IEnumerator WaitForGridAnimationRoutine(Action action=null)
    {
        Grid grid = Grid.instance;
        while (grid.IsAnimating)
        {
            yield return null;
        }
        action?.Invoke();
    }
}

public class GridDesignTemp
{
    public static readonly int[,] gridDesignTemp = new[,]
    {
        {1,1,1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,1},
        {0,0,1,1,1,1,1,0,0},
        {0,0,1,1,1,1,1,0,0},
        {0,0,1,1,1,1,1,0,0},
        {1,1,1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,1},
        {1,1,1,1,1,1,1,1,1}
    };
    // public static readonly int[,] gridDesignTemp = new[,]
    // {
    //     {1,1,1,1,1},
    //     {1,1,1,1,1},
    //     {1,1,1,1,1},
    //     {1,1,1,1,1},
    //     {1,1,1,1,1},
    //     
    // };
    public const float gridSpacing = 2.0f;
    public const float maxWidthMidPointForThisPattern = 7; 
    public const float maxHeightMidPointForThisPattern = 7; 
    public const int gridHeight = 8;
    public const int gridWidth = 8;
    
    // height width calculation ??IMPLEMENT IN EDITOR
    //     for (int i = 0; i < 8; i++)
    // {
    //     for (int j = 0; j < 8; j++)
    //     {
    //         if (_grid[i,j] == null)
    //             continue;
    //
    //         gridHeight = Mathf.Max(Mathf.Abs(_grid[i, j].transform.localPosition.y),gridHeight);
    //         Debug.Log(gridHeight);
    //         Debug.Log(_grid[i, j].transform.localPosition.y);
    //     }
    // }
}

public enum GridState
{
    UNSET,
    SET
}