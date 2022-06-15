using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchData 
{
    private List<Element> _matchedElements;
    private ElementConfig _boosterReward;
    private GridCell _spawningCell;


    public bool HasMatches
    {
        get
        {
            return _matchedElements.Count > 0;
        }
    }

    public bool HasBoosterReward
    {
        get
        {
            return BoosterReward != null;
        }
    }

    public ElementConfig BoosterReward { get { return _boosterReward; } }

    public GridCell TargetSpawningCell { get { return _spawningCell; } }

    public MatchData() 
    {
        _matchedElements = new List <Element>();
    }

    public void AddElement(Element element) 
    { 
        _matchedElements.Add(element);
    }

    public void SetReward(ElementConfig boosterReward, GridCell spawnCell) 
    { 
        _boosterReward = boosterReward;
        _spawningCell = spawnCell; 
    }

    

    internal void DestroyElements()
    {
        for (int i = 0; i < _matchedElements.Count; i++)
        {
            _matchedElements[i].DestroyElement();
        }
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
    public List<MatchData> matchData;
    public List<GridCell> patternCells;
    public int swipeId;
    public HashSet<int> dirtyColoumns;
    #endregion

    #region PUBLIC_PROPERTIES

    public bool HasMatches
    {
        get { return matchData.Count > 0; }
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
        return new MatchExecutionData(new List<MatchData>(), new List<GridCell>(), DefaultSwipeId, null, null);
    }

    #endregion

    #region CONSTRUCTOR

    public MatchExecutionData(List<MatchData> matchdata, List<GridCell> patternCellList, int swipeNumber,
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
    [SerializeField] private Grid grid;
    [SerializeField] private GridMovementAnimator gridMovementAnimator;
    [SerializeField] private GridMovementProcessor gridMovementProcessor;
    [SerializeField] private GameplayObstacleHandler gameplayObstacleHandler;
    [SerializeField] private MatchRewardHandler matchRewardHandler;
    [SerializeField] private InputManager inputManager;

    private HashSet<MatchExecutionData> activeSwipes = new HashSet<MatchExecutionData>();

    #endregion

    #region PUBLIC_METHODS

    public void Init() 
    {
        gridMovementProcessor.Init();
    }

    public void StartChecking(MatchExecutionData executionData,Dictionary<int,List<ElementAnimationData>> initialSwipeAnimationData)
    {
        StartCoroutine(IterativeCheckRoutine(executionData,initialSwipeAnimationData));
    }

    #endregion


    #region PRIVATE_VARIABLES

    private IEnumerator IterativeCheckRoutine(MatchExecutionData executionData, Dictionary<int, List<ElementAnimationData>> initialSwipeAnimationData)
    {
        Debug.LogError($"ITR {executionData.swipeId} MAIN START");
        activeSwipes.Add(executionData);
        
        FindMatches(executionData);
        yield return gridMovementAnimator.AnimateMovementRoutine(initialSwipeAnimationData);
       
        int c = 0;
        while (executionData.HasMatches)
        //if (executionData.HasMatches)
        {
            c++;
            GenerateBoosterElements(executionData);
            DestroyMatchedItems(executionData);
            yield return gridMovementAnimator.AnimateMovementRoutine(gridMovementProcessor.GenerateCollapseMovementData(executionData));
            FindMatches(executionData);
        }

        if (c <= 0)
        {
            yield return gridMovementAnimator.AnimateMovementRoutine(inputManager.SwapCells(executionData));
        }

        Debug.LogError($"ITR {executionData.swipeId} MAIN END");
        activeSwipes.Remove(executionData);

        while (activeSwipes.Count > 0) 
            yield return null;
        
        grid.UnlockCells(executionData);
    }


    private void GenerateBoosterElements(MatchExecutionData executionData) 
    {
        List<MatchData> matchDatas = executionData.matchData;
        for (int i = 0; i < matchDatas.Count; i++)
        {
            MatchData matchData = matchDatas[i];
            if (matchData.HasBoosterReward)
            {
                GenerateBoosterElement(matchData);
            }
        }
    }

    


    private void GenerateBoosterElement(MatchData matchData)
    {
        matchRewardHandler.GenerateBoosterElements(matchData);
    }

    private void DestroyMatchedItems(MatchExecutionData executionData)
    {
        List<MatchData> matchedElements = executionData.matchData;

        for (int i = 0; i < matchedElements.Count;i++)
        {
            matchedElements[i].DestroyElements();
        }
        executionData.matchData.Clear();
    }

    private void FindMatches(MatchExecutionData executionData)
    {
        

        for (int p = 0; p < patterns.Length; p++)
        {
            for (int i = 0; i < grid.GridHeight; i++)
            {
                for (int j = 0; j < grid.GridWidth; j++)
                {
                    GridCell startingCell = grid[i, j];
                    if (startingCell != null)
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
                            MatchData matchData = GenerateMatchData(executionData);
                            HitPotentialObstacles(executionData);
                            ExtractElementsToMatchData(executionData, matchData);
                            SetReward(executionData,matchData, matchPattern);
                            LockColoumns(executionData);
                        }

                        //Pattern 'p' checked successfully here
                    }
                }

            }
        }

        
    }

    private void SetReward(MatchExecutionData data,MatchData matchData, MatchPattern matchPattern)
    {
        ElementConfig boosterRewardConfig = GetInGameBoosterConfig(matchPattern);
        matchData.SetReward(boosterRewardConfig, GetRewardSpawnCell(data));
    }

    GridCell GetRewardSpawnCell(MatchExecutionData executionData) 
    { 
        List<GridCell> patternCells = executionData.patternCells;
        
        GridCell maxHeightCell = patternCells[0];
        for (int i = 0; i < patternCells.Count; i++)
        {
            GridCell targetCell = patternCells[i];
            if(targetCell == executionData.firstCell || targetCell == executionData.secondCell)
                return targetCell;

            if(targetCell.HIndex > maxHeightCell.HIndex)
                maxHeightCell = targetCell;

        }
        return maxHeightCell;
    }

    private MatchData GenerateMatchData(MatchExecutionData executionData)
    {
        MatchData matchData = new MatchData();
        executionData.matchData.Add(matchData);
        return matchData;
    }

    private void LockColoumns(MatchExecutionData executionData)
    {
        grid.LockDirtyColoumns(executionData);
        executionData.patternCells.Clear();
    }

    private void HitPotentialObstacles(MatchExecutionData executionData) {
        //TBD if safe

        gameplayObstacleHandler.CheckForNeighbourHit(executionData);
    }

    private ElementConfig GetInGameBoosterConfig(MatchPattern matchPattern) 
    { 
        return matchRewardHandler.FetchRewardConfig(matchPattern);
    }

    private MatchData ExtractElementsToMatchData(MatchExecutionData matchExecutionData,MatchData matchData)
    {
        List<GridCell> patternCells = matchExecutionData.patternCells;
        for (int k = 0; k < patternCells.Count; k++)
        {
            matchData.AddElement(patternCells[k].GetElement());
        }
        
        return matchData;
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