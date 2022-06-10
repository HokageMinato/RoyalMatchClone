using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchData 
{
    public List<List<Element>> matchedElements;
    public Element boosterElement;

    public bool HasMatches => matchedElements.Count > 0;
    public bool HasBoosterReward => boosterElement != null;


    public MatchData() 
    { 
        matchedElements = new List<List<Element>>();
    }
}

public class MatchExecutionData : IEquatable<MatchExecutionData>
{
    #region CONSTANTS

    private const int DefaultSwipeId = -99;

    #endregion

    #region PUBLIC_VARIABLES

    public GridCell firstCell;
    public GridCell secondCell;
    public MatchData matchData;
    public List<GridCell> patternCells;
    public int swipeId;
    public HashSet<int> dirtyColoumns;
    #endregion

    #region PUBLIC_PROPERTIES

    public bool HasMatches
    {
        get { return matchData.HasMatches; }
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
        return new MatchExecutionData(new MatchData(), new List<GridCell>(), DefaultSwipeId, null, null);
    }

    #endregion

    #region CONSTRUCTOR

    public MatchExecutionData(MatchData matchdata, List<GridCell> patternCellList, int swipeNumber,
        GridCell fCell, GridCell sCell)
    {
        matchData = matchdata; 
        patternCells = patternCellList;
        swipeId = swipeNumber;
        firstCell = fCell;
        secondCell = sCell;
        dirtyColoumns = new HashSet<int>();
    }

    #endregion
}

public class Matcher : Singleton<Matcher>
{
    #region PRIVATE_VARIABLES
    [SerializeField] private MatchPattern[] patterns;
    private HashSet<MatchExecutionData> activeSwipes = new HashSet<MatchExecutionData>();
    private const float FOUR_FRAME_WAITTIME = 0.64f;
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
        Debug.LogError($"ITR {executionData.swipeId} MAIN START");
        activeSwipes.Add(executionData);

        FindMatches(executionData);
        yield return new WaitForSeconds(ElementConfig.SWIPE_ANIM_TIME);
       
        int c = 0;
        while (executionData.HasMatches)
        {
            c++;
            DestroyMatchedItems(executionData);
            yield return Grid.instance.Animate(executionData);
            FindMatches(executionData);
        }

        if (c <= 0)
        {
            InputManager.instance.SwapCells(executionData);
        }

        Debug.LogError($"ITR {executionData.swipeId} MAIN END");
        activeSwipes.Remove(executionData);

        while (activeSwipes.Count > 0) 
            yield return null;
        
        Grid.instance.UnlockCells(executionData);
    }

    private void SpawnBoosters(MatchExecutionData executionData)
    {
        
    }

    private void DestroyMatchedItems(MatchExecutionData executionData)
    {
        List<List<Element>> matchedElements = executionData.matchData.matchedElements;

        for (int i = 0; i < matchedElements.Count;i++)
        {
            List<Element> sameElementsList = matchedElements[i];
            for (int j = 0; j < sameElementsList.Count; j++)
            {
                Element elementToBeDestroyed = sameElementsList[j];
                elementToBeDestroyed.DestroyElement();
            }

        }

        executionData.matchData.matchedElements.Clear();
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
                        //ExtractPatternCells(startingCell, matchPattern, i, j, executionData);
                        PatternCompareUtility.GetMatchedPatternCellsNonAlloc(executionData.patternCells,matchPattern, i, j);
                        

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
                            HitPotentialObstacles(executionData);
                            ExtractElementsToDestroyList(executionData);

                        }

                        //Pattern 'p' checked successfully here
                    }
                }


            }
        }

        
    }

    private void HitPotentialObstacles(MatchExecutionData executionData) {
        //TBD if safe

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

        matchExecutionData.matchData.matchedElements.Add(sameElementList);
        grid.LockDirtyColoumns(matchExecutionData);
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

        GridCell initialCell = patternCells[0];
        for (int i = 1; i < patternCells.Count; i++)
        {
            if (!initialCell.ReadElement().Equals(patternCells[i].ReadElement())) 
            {
                patternCells.Clear();
                return false;
            }
        }

        return true;
    }
    #endregion
}