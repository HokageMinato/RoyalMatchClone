using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameplayBoosterHitData 
{
    public List<Element> hitElements = new List<Element>();
    public List<Element> AnotherBoosterHitElements = new List<Element>();
    public List<GridCell> tobeLockedCells = new List<GridCell>();

    public List<Element> GetAndEmptyHitElements() 
    { 
        List<Element> tempHitElements = new List<Element>(hitElements);
        hitElements.Clear();
        return tempHitElements;
    }

    public void AddElement(Element element) 
    {
        Debug.Log($"{element == null} -- {element.ElementConfig == null}  -- {ElementFactory.instance == null}");
        
        if(IsBoosterConfig(element.ElementConfig))
            AnotherBoosterHitElements.Add(element);

        if(!hitElements.Contains(element))
            hitElements.Add(element);   

    }

    public bool HasPendingBoosterHits() 
    { 
        return AnotherBoosterHitElements.Count > 0;
    }

    private bool IsBoosterConfig(ElementConfig elemetConfig) 
    { 
        return ElementFactory.instance.IsBooster(elemetConfig);
    }
}
public class MatchData 
{
    private List<Element> _matchedElements;
    private Element _boosterRewardForMatch;
    private GridCell _spawningCell;


    public bool HasMatches
    {
        get
        {
            return _matchedElements.Count > 0;
        }
    }

    public bool HasBoosterRewardForPatternMatch
    {
        get
        {
            return BoosterRewardForPatternMatch != null;
        }
    }

    
    public Element BoosterRewardForPatternMatch { get { return _boosterRewardForMatch; } }

    public GridCell TargetSpawningCell { get { return _spawningCell; } }

    public MatchData() 
    {
        _matchedElements = new List <Element>();
    }

    public void AddElement(Element element) 
    { 
        _matchedElements.Add(element);
    }

    public void SetReward(Element boosterReward, GridCell spawnCell) 
    {
        if (boosterReward == null)
            return;

        _boosterRewardForMatch = boosterReward;
        _spawningCell = spawnCell; 
        _boosterRewardForMatch.gameObject.SetActive(false);
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
    public GameplayBoosterHitData boosterHitData;
    public int swipeId;
    public HashSet<int> dirtyColoumns;
    public GridCell boosterCell=null;
    #endregion

    #region PUBLIC_PROPERTIES

    public bool HasMatches
    {
        get { return matchData.Count > 0; }
    }


    public Element FirstElement => firstCell?.ReadElement();
    public Element SecondElement => secondCell?.ReadElement();
    
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
        boosterHitData = new GameplayBoosterHitData();
    }

    #endregion
}



public class MatchSwipeHandler : MonoBehaviour, IMatchHandler
{
    #region PRIVATE_VARIABLES
    private HashSet<MatchExecutionData> activeSwipes = new HashSet<MatchExecutionData>();
    private Gridd grid;
    #endregion

    #region PUBLIC_METHODS
    public void Init() 
    {
        grid = Gridd.instance;
    }

   

    public void HandleSwipe(MatchExecutionData matchExecutionData)
    {
        StartCoroutine(MatchRoutine(matchExecutionData));
    }

    public bool CanHandleSwipe(MatchExecutionData matchExecutionData)
    {
        return true;
    }

    #endregion


    #region PRIVATE_VARIABLES

    private IEnumerator MatchRoutine(MatchExecutionData executionData)
    {
        MatchFinder matchFinder = MatchFinder.instance;
        GridMovementAnimator gridMovementAnimator = GridMovementAnimator.instance;

        Debug.LogError($"ITR {executionData.swipeId} MAIN START");
        activeSwipes.Add(executionData);

        yield return gridMovementAnimator.AnimateCellSwap(executionData);

        matchFinder.FindMatches(executionData);
        grid.LockDirtyColoumns(executionData);

        bool isReSwap = !executionData.HasMatches;

        if (isReSwap) // No Matches
        {
            yield return gridMovementAnimator.AnimateCellSwap(executionData);
        }
        else 
        {
            yield return matchFinder.CollapseRoutine(executionData);
        }


        Debug.LogError($"ITR {executionData.swipeId} MAIN END");
        activeSwipes.Remove(executionData);

        while (activeSwipes.Count > 0) 
            yield return null;
        
        grid.UnlockCells(executionData);
    }

    










    #endregion
}