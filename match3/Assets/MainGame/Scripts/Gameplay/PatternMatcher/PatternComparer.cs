using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternComparer :Singleton<PatternComparer>
{


    //public List<GridCell> GetMatchedPatternCells(MatchPattern matchPattern,int i, int j) 
    //{
    //    Grid grid = Grid.instance;
    //    List<GridCell> outputList = new List<GridCell>();

    //    //  Debug.Log($"<Matcher> Checking {matchPattern.patternName} at cell {startingCell.gameObject.name}");

    //    for (int k = 0; k < matchPattern.Length; k++) //Generate a list of cell from patterm
    //    {
    //        IndexPair offsetIndexPair = matchPattern[k];
    //        int iPaired = i + offsetIndexPair.I_Offset;
    //        int jPaired = j + offsetIndexPair.J_Offset;


    //        if (iPaired >= grid.GridHeight || jPaired >= grid.GridWidth ||
    //            grid[iPaired, jPaired] == null || grid[iPaired, jPaired].IsEmpty)
    //        {
    //            // Either grid geometry doesn't allow further check or previous pattern locked and extracted the cell thus current pattern will fail,
    //            // so we terminate execution instantly and clear the extractList;
    //           // outputList.Clear();

    //            //editor logging
    //            //   Debug.Log($"<Matcher> Terminating check due i{iPaired} j{jPaired} >= {grid.GridHeight} {grid.GridWidth}");
    //            //  Debug.Log($"<Matcher> OR");
    //            // Debug.Log($"<Matcher> Terminating check due to no cell present at or is Empty {i}{j}");
    //            //end logging

    //            return null;
    //        }

    //        GridCell cellOfPattern = grid[iPaired, jPaired];
    //        outputList.Add(cellOfPattern);
    //    }

    //    return outputList;

    //}


    public void GetAllPatternCellsNonAlloc(List<GridCell> outputList, MatchPattern matchPattern, int hI, int wJ) 
    {
        Grid grid = Grid.instance;

        //  Debug.Log($"<Matcher> Checking {matchPattern.patternName} at cell {startingCell.gameObject.name}");

        for (int k = 0; k < matchPattern.Length; k++) //Generate a list of cell from patterm
        {
            IndexPair offsetIndexPair = matchPattern[k];
            int iPaired = hI + offsetIndexPair.I_Offset;
            int jPaired = wJ + offsetIndexPair.J_Offset;


            if (IsOutOfBounds(iPaired, jPaired))
            {
                // Either grid geometry doesn't allow further check or previous pattern locked and extracted the cell thus current pattern will fail,
                // so we just add the cells which can be added


                //editor logging
                //   Debug.Log($"<Matcher> Terminating check due i{iPaired} j{jPaired} >= {grid.GridHeight} {grid.GridWidth}");
                //  Debug.Log($"<Matcher> OR");
                // Debug.Log($"<Matcher> Terminating check due to no cell present at or is Empty {i}{j}");
                //end logging
                continue;
            }

            GridCell cellOfPattern = grid[iPaired, jPaired];
            outputList.Add(cellOfPattern);
        }
    }

    private static bool IsOutOfBounds(int iPaired, int jPaired)
    {
        Grid grid = Grid.instance;
        return iPaired >= grid.GridHeight || jPaired >= grid.GridWidth || iPaired < 0 || jPaired < 0;
    }

    public void GetMatchedPatternCellsNonAlloc(List<GridCell> outputList,MatchPattern matchPattern, int hI, int wJ)
    {
        Grid grid = Grid.instance;

        //  Debug.Log($"<Matcher> Checking {matchPattern.patternName} at cell {startingCell.gameObject.name}");

        for (int k = 0; k < matchPattern.Length; k++) //Generate a list of cell from patterm
        {
            IndexPair offsetIndexPair = matchPattern[k];
            int iPaired = hI + offsetIndexPair.I_Offset;
            int jPaired = wJ + offsetIndexPair.J_Offset;


            if (IsOutOfBounds(iPaired,jPaired) ||
                grid[iPaired, jPaired] == null || grid[iPaired, jPaired].IsEmpty)
            {
                // Either grid geometry doesn't allow further check or previous pattern locked and extracted the cell thus current pattern will fail,
                // so we terminate execution instantly and clear the extractList;
                outputList.Clear();

                //editor logging
                //   Debug.Log($"<Matcher> Terminating check due i{iPaired} j{jPaired} >= {grid.GridHeight} {grid.GridWidth}");
                //  Debug.Log($"<Matcher> OR");
                // Debug.Log($"<Matcher> Terminating check due to no cell present at or is Empty {i}{j}");
                //end logging
                return;
            }

            GridCell cellOfPattern = grid[iPaired, jPaired];
            outputList.Add(cellOfPattern);
        }

    }


}
