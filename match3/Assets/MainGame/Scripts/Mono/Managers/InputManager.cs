using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : Singleton<InputManager>
{
    [SerializeField] private GridCell _firstCell;
    [SerializeField] private GridCell _secondCell;
    private int swipeNumber = 0;

    public void SetFirstCell(GridCell firstCell)
    {
        if(firstCell.IsEmpty)
            return;
        
        _firstCell = firstCell;
    }

   

    public void SetSecondCell(GridCell secondCell)
    {

        if (_firstCell== null)
            return;

        if (_firstCell.Equals(secondCell))
            return;

        if (secondCell.IsEmpty)
        {
            _firstCell = null;
            _secondCell = null;
            return;
        }

        _secondCell = secondCell;
        ValidateMove();
    }

   
    private void ValidateMove()           
    {
        if (IsFirstCellAssigned())
        {
            if (AreNeighbours(_firstCell,_secondCell))
            {
                OnValidMove();
            }
        }
    }

    private void OnValidMove()
    {
        MatchExecutionData matchExecutionData = new MatchExecutionData(new List<MatchData>(),new List<GridCell>(),swipeNumber,_firstCell,_secondCell);
        swipeNumber++;

        matchExecutionData.firstCell.SetExecutionData(matchExecutionData);
        matchExecutionData.secondCell.SetExecutionData(matchExecutionData);
        _firstCell = _secondCell = null;
        Matcher.instance.StartMatching(matchExecutionData, SwapCells(matchExecutionData));
    }

    public Dictionary<int, List<ElementAnimationData>> SwapCells(MatchExecutionData matchExecutionData)
    {
        GridCell firstCell = matchExecutionData.firstCell;
        GridCell secondCell = matchExecutionData.secondCell;

        Element secondElement = secondCell.GetElement();
        Element firstElement = firstCell.GetElement();
        
        Dictionary<int,List<ElementAnimationData>> initialSwipeAnimationData = new Dictionary<int, List<ElementAnimationData>>() 
        {
            {0,GenerateMoveAnimationData(firstElement,secondElement,firstCell,secondCell,matchExecutionData)},
            {1,GenerateMoveAnimationData(secondElement,firstElement,secondCell,firstCell,matchExecutionData)}
        };
        return initialSwipeAnimationData;
    }

    private List<ElementAnimationData> GenerateMoveAnimationData(Element currentElement,Element otherElement, GridCell currentCell,GridCell otherCell,MatchExecutionData data) 
    {
        List<ElementAnimationData> elemAnimation = new List<ElementAnimationData>();
        currentCell.SetElement(otherElement);
        elemAnimation.Add(new ElementAnimationData(currentElement, currentCell, otherCell, data, currentCell.HIndex));
        return elemAnimation;
    }

   

    private bool IsFirstCellAssigned()
    {
        return _firstCell != null;
    }

    private bool AreNeighbours(GridCell firstCell, GridCell secondCell)
    {

        bool isVerticalPositionSame = firstCell.HIndex == secondCell.HIndex;
        bool isHorizontalPositionSame = firstCell.WIndex == secondCell.WIndex;
        int cellDistance;

        if (isHorizontalPositionSame)
        {
            cellDistance = Mathf.Abs(firstCell.HIndex - secondCell.HIndex);
        }
        else if (isVerticalPositionSame)
        {
            cellDistance = Mathf.Abs(firstCell.WIndex - secondCell.WIndex);
        }
        else
        {
            return false;
        }

        bool areNeighbours = cellDistance <= 1 && (isVerticalPositionSame || isHorizontalPositionSame);

        return areNeighbours;
    }

}
