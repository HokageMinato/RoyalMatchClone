using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchFinder : Singleton<MatchFinder>
{
    [SerializeField] private MatchPattern[] patterns;
    [SerializeField] private GridMovementAnimator gridMovementAnimator;
    [SerializeField] private GridMovementProcessor gridMovementProcessor;
    
    private MatchGameplayBoosterRewardHandler matchGameplayBoosterRewardHandler;

    public void Init() 
    {
        matchGameplayBoosterRewardHandler = MatchGameplayBoosterRewardHandler.instance;
        gridMovementProcessor.Init();
    }

    
    public IEnumerator CollapseRoutine(MatchExecutionData executionData)
    {
        while (executionData.HasMatches)
        {
            SetMatchRewardBoosterElements(executionData);
            DestroyMatchedItems(executionData);
            yield return gridMovementAnimator.AnimateMovementRoutine(gridMovementProcessor.GenerateCollapseMovementData(executionData));
            FindMatches(executionData);
        }

    }

    public IEnumerator BoosterCollapseRoutine(MatchExecutionData executionData)
    {
        yield return gridMovementAnimator.AnimateMovementRoutine(gridMovementProcessor.GenerateCollapseMovementData(executionData));
        yield return CollapseRoutine(executionData);
    }   

    public void FindMatches(MatchExecutionData executionData)
    {
        Gridd grid = Gridd.instance;

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
                        PatternCompareUtility.GetMatchedPatternCellsNonAlloc(executionData.patternCells, matchPattern, i, j);


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
                            MatchData matchData = GenerateMatchData(executionData, matchPattern);
                            HitPotentialObstacles(executionData);
                            ExtractElementsToMatchData(executionData, matchData);
                            LockColoumns(executionData);
                        }

                        //Pattern 'p' checked successfully here
                    }
                }
            }
        }


    }


    private void DestroyMatchedItems(MatchExecutionData executionData)
    {
        List<MatchData> matchedElements = executionData.matchData;

        for (int i = 0; i < matchedElements.Count; i++)
        {
            matchedElements[i].DestroyElements();
        }
        executionData.matchData.Clear();
    }

    private void SetMatchRewardBoosterElements(MatchExecutionData executionData)
    {
        List<MatchData> matchDatas = executionData.matchData;
        for (int i = 0; i < matchDatas.Count; i++)
        {
            MatchData matchData = matchDatas[i];
            if (matchData.HasBoosterRewardForPatternMatch)
            {
                SetMatchRewardBoosterElement(matchData);
            }
        }
    }

    private void SetMatchRewardBoosterElement(MatchData matchData)
    {
        GridCell targetCell = matchData.TargetSpawningCell;
        Element rewardElement = matchData.BoosterRewardForPatternMatch;
        rewardElement.gameObject.SetActive(true);
        targetCell.SetElement(rewardElement);
        rewardElement.transform.localPosition = targetCell.transform.localPosition;
    }

    private MatchData GenerateMatchData(MatchExecutionData executionData, MatchPattern matchPattern)
    {
        MatchData matchData = new MatchData();
        executionData.matchData.Add(matchData);

        Element boosterReward = matchGameplayBoosterRewardHandler.GetElementForPattern(matchPattern);
        matchData.SetReward(boosterReward, GetRewardSpawnCell(executionData));

        return matchData;
    }

    private void HitPotentialObstacles(MatchExecutionData executionData)
    {
        //TBD if safe
        GameplayObstacleHandler.instance.CheckForNeighbourHit(executionData);
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

    private MatchData ExtractElementsToMatchData(MatchExecutionData matchExecutionData, MatchData matchData)
    {
        List<GridCell> patternCells = matchExecutionData.patternCells;
        for (int k = 0; k < patternCells.Count; k++)
        {
            matchData.AddElement(patternCells[k].GetElement());
        }

        return matchData;
    }

    private void LockColoumns(MatchExecutionData executionData)
    {
        Gridd.instance.LockDirtyColoumns(executionData);
        executionData.patternCells.Clear();
    }

    GridCell GetRewardSpawnCell(MatchExecutionData executionData)
    {
        List<GridCell> patternCells = executionData.patternCells;

        GridCell maxHeightCell = patternCells[0];
        for (int i = 0; i < patternCells.Count; i++)
        {
            GridCell targetCell = patternCells[i];
            if (targetCell == executionData.firstCell || targetCell == executionData.secondCell)
                return targetCell;

            if (targetCell.HIndex > maxHeightCell.HIndex)
                maxHeightCell = targetCell;

        }
        return maxHeightCell;
    }

}
