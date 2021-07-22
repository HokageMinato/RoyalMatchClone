using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridEditorUtility : MonoBehaviour
{
    #region EDITOR_VARS
    [SerializeField] EditorGridCell gridPrefab;
    [SerializeField] private int gridHeight;
    [SerializeField] private int gridWidth;
    [SerializeField] private float widthSpacing;
    [SerializeField] private float heightSpacing;
    [SerializeField] private Transform gridPivot;
    #endregion

    #region PRIVATE_VARS
    private EditorGridCell[,] grid;
    public bool IsGridGenerated => grid != null;
    
    #endregion

    #region PUBLIC_METHODS
    public void GenerateGrid()
    {
        if (ValidInputEntered())
        {
            grid = new EditorGridCell[gridWidth,gridHeight];
            Vector3 pivot = gridPivot.localPosition;
            
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = Instantiate(gridPrefab, transform);
                    EditorGridCell cell = grid[i, j];
                    cell.transform.localPosition = pivot;

                    pivot.x += widthSpacing;
                }

                pivot.y += heightSpacing;
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
       
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                DestroyImmediate(grid[i,j].gameObject);
            }
        }

        grid = null;
    }


    public void OnValidate()
    {
        SetSizes();
    }

    #endregion
    
    #region PRIVATE_METHODS

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
                EditorGridCell cell = grid[i, j];
                cell.transform.localPosition = pivot;
                 pivot.x += widthSpacing;
            }

            pivot.y += heightSpacing;
            pivot.x = gridPivot.transform.localPosition.x;
        }  
    }
    #endregion

}
