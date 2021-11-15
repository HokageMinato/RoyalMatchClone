using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridColoumnCollapser : MonoBehaviour
{

    [ContextMenu("Test")]
    public void CollapseColomuns() {

        Grid grid = Grid.instance;

        for (int i = grid.GridWidth - 1; i >= 0; i--)
        {
            ShiftCellDownwards(i);
        }


      

    }




    private void ShiftCellDownwards(int gI) {

        Grid grid = Grid.instance;
        
        List<int[]> fromToIndexPairs = new List<int[]>();

        fromToIndexPairs.Add(new int[2]);
        fromToIndexPairs[fromToIndexPairs.Count - 1][0] = 0;
        fromToIndexPairs[fromToIndexPairs.Count - 1][1] = 0;

        for (int k = 0; k < grid.GridHeight; k++)
        {
            if (grid[k,gI] == null || !grid[k,gI].IsBlocked)
            {
                fromToIndexPairs[fromToIndexPairs.Count - 1][1] = k;
            }
            else if (grid[k,gI].IsBlocked && k < grid.GridHeight - 1)
            {
                fromToIndexPairs.Add(new int[2]);
                fromToIndexPairs[fromToIndexPairs.Count - 1][0] = k + 1;
                fromToIndexPairs[fromToIndexPairs.Count - 1][1] = k + 1;
            }

        }

        for (int k = 0; k < fromToIndexPairs.Count; k++)
        {
            int start = fromToIndexPairs[k][0];
            int endt = fromToIndexPairs[k][1];


            if (start == endt && grid[start,gI]!=null)
            {   
                grid[start, gI].renderer.color = Color.red;
                continue;
            }

            Color color = Random.ColorHSV();

            for (int j = endt; j >= start; j--)
            {

                GridCell currentCell = grid[j, gI];
                
                if (currentCell == null)
                    continue;
                
                currentCell.renderer.color = color;
                if (currentCell.IsEmpty)
                {
                    GridCell filledCell = GetTopMostFilledCell(start, j);

                    if (filledCell == null)
                    {
                        break;
                    }
                    currentCell.SetElement(filledCell.GetElement());
                }
            }
        }

        GridCell GetTopMostFilledCell(int st, int ed)
        {

            for (int tpI = ed; tpI >= st; tpI--)
            {
                GridCell currentCell = grid[tpI, gI];
                if (currentCell!=null && !currentCell.IsEmpty)
                {
                    return currentCell;
                }
            }
            return null;

        }

    }


   
    

}
