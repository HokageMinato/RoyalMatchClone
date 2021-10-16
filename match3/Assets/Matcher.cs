﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchExecutionData
{
    #region CONSTANTS

    private const int DefaultSwipeId = -99;

    #endregion

    #region PUBLIC_VARIABLES

    public GridCell firstCell;
    public GridCell secondCell;
    public List<List<Element>> matchedElements;
    public List<GridCell> patternCells;
    public int swipeId;

    #endregion

    #region PUBLIC_PROPERTIES

    public bool HasMatches
    {
        get { return matchedElements.Count > 0; }
    }

    #endregion

    #region PUBLIC_METHODS

    public bool Equals(MatchExecutionData obj)
    {
        if (obj == null)
            return false;

        if (obj.swipeId == DefaultSwipeId || swipeId == DefaultSwipeId)
        {
            return true;
        }

        return obj.swipeId == swipeId;
    }

    public override string ToString()
    {
        return $"Execution data id:{swipeId}";
    }


    public static MatchExecutionData GetDefaultExecutionData()
    {
        return new MatchExecutionData(new List<List<Element>>(), new List<GridCell>(), DefaultSwipeId, null, null);
    }


    
    #endregion

    #region CONSTRUCTOR

    public MatchExecutionData(List<List<Element>> matchedElementList, List<GridCell> patternCellList, int swipeNumber,
        GridCell fCell, GridCell sCell)
    {
        matchedElements = matchedElementList;
        patternCells = patternCellList;
        swipeId = swipeNumber;
        firstCell = fCell;
        secondCell = sCell;
    }

    #endregion
}

public class Matcher : Singleton<Matcher>
{
    #region PRIVATE_VARIABLES
    [SerializeField] private MatchPattern[] patterns;
    private List<MatchExecutionData> activeThreads = new List<MatchExecutionData>();
    #endregion
    
    #region PUBLIC_METHODS

    public void StartChecking(MatchExecutionData executionData)
    {
        StartCoroutine(IterativeCheckRoutine(executionData));
    }



    #endregion

    #region PRIVATE_VARIABLES


   
    private IEnumerator IterativeCheckRoutine(MatchExecutionData executionData)
    {
        activeThreads.Add(executionData);
        FindMatches(executionData);
        yield return WaitForGridAnimationRoutine(executionData);

        if (!executionData.HasMatches)
        {
            //reswap cells;
            InputManager.instance.SwapCells(executionData);
            activeThreads.Remove(executionData);
            yield break;
        }
      

        Grid grid = Grid.instance;
        while (executionData.HasMatches)
        {
            DestroyMatchedItems(executionData);
            grid.CollapseColoumns(executionData);
            yield return WaitForGridAnimationRoutine(executionData);
            FindMatches(executionData);
        }

        activeThreads.Remove(executionData);

        while (activeThreads.Count > 0)
            yield return null;


        grid.UnlockCells(executionData);
    }


    private IEnumerator WaitForGridAnimationRoutine(MatchExecutionData executionData, Action action = null)
    {
        yield return new WaitForSeconds(Element.SWIPE_ANIM_TIME);
       
        action?.Invoke();
    }


    private void DestroyMatchedItems(MatchExecutionData executionData)
    {
        List<List<Element>> matchedElements = executionData.matchedElements;

        for (int i = 0; i < matchedElements.Count;)
        {
            List<Element> sameElementsList = matchedElements[i];
            for (int j = 0; j < sameElementsList.Count; j++)
            {
                Destroy(sameElementsList[j].gameObject);
            }

            matchedElements.RemoveAt(i);
        }
    }

    private void FindMatches(MatchExecutionData executionData)
    {
        Grid grid = Grid.instance;

        for (int i = 0; i < grid.GridHeight; i++)
        {
            for (int j = 0; j < grid.GridWidth; j++)
            {
                GridCell startingCell = grid[i, j];
                if (startingCell != null)
                {
                    for (int p = 0; p < patterns.Length; p++)
                    {
                        //Matching started for a single pattern 'p'
                        MatchPattern matchPattern = patterns[p];

                        //We extract the cells according to pattern specified offsets
                        ExtractPatternCells(startingCell, matchPattern, i, j, executionData);

                        if (!IsExtractionValid(executionData))
                        {
                            //Incase the geometry of grid doesnt allow us to continue,
                            // we skip the pattern for this 'Starting Cell' and check for next pattern 'p' at the 'Starting Cell'.
                            continue;
                        }

                        //At this point we have cells with current pattern 'p', We can now safely check if 
                        //all the listed elements are same or not.
                        if (DoesSelectedCellsHaveSameElements(executionData))
                        {
                            //Debug.Log($"<Matcher> Cell count {_patternCells.Count}");
                            HitPotentialObstacles(executionData);
                            ExtractElementsToDestroyList(executionData);
                            // Debug.Log("--------------------------------------------------------------");
                            //Debug.Log(".");
                            //Debug.Log(".");
                        }

                        //Pattern 'p' checked successfully here
                    }
                }
            }
        }

        //Debug.Log($"Total patterns detected {_matchedElements.Count}");
    }

    private void HitPotentialObstacles(MatchExecutionData executionData) {
        
        GameplayObstacleHandler.instance.CheckForNeighbourHit(executionData);

    }

    private void ExtractElementsToDestroyList(MatchExecutionData matchExecutionData)
    {
        Grid grid = Grid.instance;
        List<GridCell> patternCells = matchExecutionData.patternCells;
        List<Element> sameElementList = new List<Element>();
        for (int k = 0; k < patternCells.Count; k++)
        {
            Element element = patternCells[k].GetElement();
            sameElementList.Add(element);
        }

        grid.LockDirtyColoumns(matchExecutionData);
        matchExecutionData.matchedElements.Add(sameElementList);
        patternCells.Clear();
    }

    private bool IsExtractionValid(MatchExecutionData executionData)
    {
        List<GridCell> patternCells = executionData.patternCells;
        return (patternCells.Count > 0);
    }

    private bool DoesSelectedCellsHaveSameElements(MatchExecutionData matchExecutionData)
    {
        List<GridCell> patternCells = matchExecutionData.patternCells;

        Element startingElement = patternCells[0].ReadElement();
        for (int k = 1; k < patternCells.Count; k++)
        {
            Element patternCellElement = patternCells[k].ReadElement();
            if (!patternCellElement.IsSame(startingElement))
            {
                //      Debug.Log($"<Matcher> Terminating check due to different elements present at {_patternCells[k].gameObject.name} or is empty");
                patternCells.Clear();
                return false;
            }
        }

        return true;
    }

    private void ExtractPatternCells(GridCell startingCell, MatchPattern matchPattern, int i, int j,
        MatchExecutionData matchExecutionData)
    {
        List<GridCell> patternCells = matchExecutionData.patternCells;
        Grid grid = Grid.instance;

        //  Debug.Log($"<Matcher> Checking {matchPattern.patternName} at cell {startingCell.gameObject.name}");

        for (int k = 0; k < matchPattern.Length; k++) //Generate a list of cell from patterm
        {
            IndexPair offsetIndexPair = matchPattern[k];
            int iPaired = i + offsetIndexPair.I_Offset;
            int jPaired = j + offsetIndexPair.J_Offset;


            if (iPaired >= grid.GridHeight || jPaired >= grid.GridWidth ||
                grid[iPaired, jPaired] == null || grid[iPaired, jPaired].IsEmpty)
            {
                // Either grid geometry doesn't allow further check or previous pattern locked and extracted the cell thus current pattern will fail,
                // so we terminate execution instantly and clear the extractList;
                patternCells.Clear();

                //editor logging
                //   Debug.Log($"<Matcher> Terminating check due i{iPaired} j{jPaired} >= {grid.GridHeight} {grid.GridWidth}");
                //  Debug.Log($"<Matcher> OR");
                // Debug.Log($"<Matcher> Terminating check due to no cell present at or is Empty {i}{j}");
                //end logging

                return;
            }

            GridCell cellOfPattern = grid[iPaired, jPaired];
            patternCells.Add(cellOfPattern);
        }
    }
    #endregion
}