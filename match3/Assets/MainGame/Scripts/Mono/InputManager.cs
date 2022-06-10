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
        MatchExecutionData matchExecutionData = new MatchExecutionData(new MatchData(),new List<GridCell>(),swipeNumber,_firstCell,_secondCell);
        matchExecutionData.firstCell.SetExecutionData(matchExecutionData);
        matchExecutionData.secondCell.SetExecutionData(matchExecutionData);
        _firstCell = _secondCell = null;
        SwapCells(matchExecutionData);
        swipeNumber++;
        Matcher.instance.StartChecking(matchExecutionData);
    }

    public void SwapCells(MatchExecutionData matchExecutionData)
    {
        Element firstElement = matchExecutionData.firstCell.GetElement();
        Element secondElement = matchExecutionData.secondCell.GetElement();

        matchExecutionData.firstCell.SetElement(secondElement);
        matchExecutionData.secondCell.SetElement(firstElement);

        secondElement.AnimateToCell(matchExecutionData.firstCell.transform);
        firstElement.AnimateToCell(matchExecutionData.secondCell.transform);
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
