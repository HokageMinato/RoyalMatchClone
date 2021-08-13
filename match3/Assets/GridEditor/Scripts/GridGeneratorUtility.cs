using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class GridGeneratorUtility : MonoBehaviour
{
    #region EDITOR_VARS
    [SerializeField] private GridCell gridPrefab;
    [SerializeField] private GameObject seperatorPrefab;
    [SerializeField] private int gridHeight;
    [SerializeField] private int gridWidth;
    [SerializeField] private float widthSpacing;
    [SerializeField] private float heightSpacing;
    [SerializeField] private Transform gridPivot;
    #endregion

    #region PUBLIC_VARS
    public bool IsGridGenerated => grid != null;
    #if UNITY_EDITOR
    public List<GridCell> cells;
    public List<GameObject> seperators;
    #endif
    #endregion
    
    #region PRIVATE_VARS
    private GridCell[,] grid;
    #endregion
    
    
    #region PUBLIC_METHODS
    public void GenerateGrid()
    {
        if (!GetComponent<Grid>())
        {
            gameObject.AddComponent<Grid>();
        }

        if (ValidInputEntered())
        {
            grid = new GridCell[gridHeight,gridWidth];
            Vector3 pivot = gridPivot.localPosition;
            
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = Instantiate(gridPrefab, transform);
                    GridCell cell = grid[i, j];
                    cell.gameObject.name = $"({i},{j})";
                    cell.transform.localPosition = pivot;
                    cells.Add(cell);
                    cell.Init(i,j);
                    pivot.x += widthSpacing;
                }

                seperators.Add(Instantiate(seperatorPrefab, transform));
                pivot.y -= heightSpacing;
                pivot.x = gridPivot.transform.localPosition.x;
            }
        }
        else
        {
            Debug.LogError("Enter Valid Inputs");
        }
    }


    public void ResetGrid()
    {
        foreach (GridCell gridCell in cells)
        {
            DestroyImmediate(gridCell.gameObject);
        }

        foreach (GameObject seperator in seperators)
        {
            DestroyImmediate(seperator.gameObject);
        }
        seperators.Clear();
        cells.Clear();
        grid = null;
    }


    public void SetSources()
    {
        FillArray();
        DiscardPreviousSources();
        AssignNewSources();
        
        GetComponent<Grid>().SetGrid(cells,gridHeight,gridWidth);
        
    }

    private void DiscardPreviousSources()
    {
        for (int i = 0; i < cells.Count; i++)
        {
            cells[i].elementSource = null;
        }
    }

    private void AssignNewSources()
    {
        for (int i = 0; i < grid.GetLength(0) - 1; i++)
        {
            List<GridCell> activeCellList = new List<GridCell>();

            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if (!grid[i, j].gameObject.activeSelf)
                    activeCellList.Add(grid[i, j]);
            }

            for (int j = activeCellList.Count; j > 0; j++)
            {
                activeCellList[j].elementSource = activeCellList[j - 1];
            }
        }
    }

    public void OnValidate()
    {
        SetSizes();
        FillArray();
    }

    #endregion
    
    #region PRIVATE_METHODS

    private void FillArray()
    {
        if (cells.Count <= 0)
        {
            grid = null;
            return;
        }

        if (grid == null)
        {
            grid = new GridCell[gridWidth,gridHeight];
            int counter = 0;
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = cells[counter];
                }
            }
        }

    }

    private bool ValidInputEntered()
    {
        bool isInputValid = true;

        isInputValid = gridHeight > 0 && gridWidth > 0;
        
        return isInputValid;
    }

    private void SetSizes()
    {
        if(!IsGridGenerated) return; 
        
        Vector3 pivot = gridPivot.localPosition;
        
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                GridCell cell = grid[i, j];
                cell.transform.localPosition = pivot;
                 pivot.x += widthSpacing;
            }

            pivot.y += heightSpacing;
            pivot.x = gridPivot.transform.localPosition.x;
        }  
    }
    #endregion

    
}
