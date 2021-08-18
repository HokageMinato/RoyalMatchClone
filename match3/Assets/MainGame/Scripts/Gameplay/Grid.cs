using System.Collections.Generic;
using UnityEngine;
 
public class Grid : Singleton<Grid>
{

    #region PUBLIC_VARIABLES
    public List<GridCell> gridList;
    public GridCell gridPrefab;
    public Element[] elementPrefab;
    public int gridHeight;
    public int gridWidth;
    #endregion


   
    
    
    #region PRIVATE_VARIABLES
    private GridCell[,] _grid;
    #endregion

    #region UNITY_CALLBACKS
    private void Awake()
    {
        CreateGrid();
    }
    #endregion


    #region PRIVATE_METHODS
    private void CreateGrid()
    {
        int c = 0;
        _grid = new GridCell[9, 9];
       
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (GridDesignTemp.gridDesignTemp[i, j] == 0)
                    continue;
                 
                CreateCellAt(j, i);
                FillInitialElementAt(j, i,c);
                c++;
            }
        }

    }

    private void CreateCellAt(int j, int i)
    {
        GridCell cell = Instantiate(gridPrefab, transform);
        Vector3 newPosition = transform.position + new Vector3(j, -i) * GridDesignTemp.gridSpacing;
        newPosition.x -= GridDesignTemp.maxWidthMidPointForThisPattern; //To make sure gridPivot is in center
        cell.transform.position = newPosition;
        _grid[i, j] = cell;
        cell.gameObject.name = $"({i},{j})";
    }

    private void FillInitialElementAt(int j, int i,int c)
    {
        GridCell parentCell = _grid[i, j];
        Element element = Instantiate(elementPrefab[Random.Range(0,elementPrefab.Length)],parentCell.transform);
        
        
        Transform elementTransform = element.transform;
        elementTransform.localPosition = new Vector3(0f,15f,0f);
        element.gameObject.name = c.ToString();
        element.SetHolder(parentCell);
    }

    #endregion

    #region PUBLIC_METHODS
    public void SetGrid(List<GridCell> gridCells, int height, int width)
    {
        gridList = gridCells;
        gridHeight = height;
        gridWidth = width;
    }
 
     #endregion


}

public class GridDesignTemp
{
    public static readonly int[,] gridDesignTemp = new[,]
    {
        {1,1,1,1,1,1,1,1,1},
        {0,0,0,1,1,1,0,0,0},
        {0,0,1,1,1,1,1,0,0},
        {0,0,1,1,1,1,1,0,0},
        {1,1,1,1,1,1,1,1,1},
        {0,0,0,1,1,1,0,0,0},
        {0,0,0,1,1,1,0,0,0},
        {0,0,0,1,1,1,0,0,0},
        {0,0,0,1,1,1,0,0,0}
    };
    public const float gridSpacing = 2.0f;
    public const float maxWidthMidPointForThisPattern = 8; 
    
}

