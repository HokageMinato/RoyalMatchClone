using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class Grid : MonoBehaviour
{
    public List<GridCell> gridList;
    private GridCell[,] _grid;
    
    public int gridHeight;
    public int gridWidth;


    private void Awake()
    {
        InitializeGrid();
    }


    private void InitializeGrid()
    {
        int hCounter=0;
        int wCounter=0;
        _grid = new GridCell[gridHeight, gridWidth];
        for (int i = 0; i < gridList.Count; i++)
        {
            _grid[hCounter, wCounter] = gridList[i];
            wCounter++;

            if (wCounter >= gridWidth)
            {
                hCounter++;
                wCounter = 0;
            }

        }
    }

    public void SetGrid(List<GridCell> gridCells, int height, int width)
    {
        gridList = gridCells;
        gridHeight = height;
        gridWidth = width;
    }
    
    
    

}
