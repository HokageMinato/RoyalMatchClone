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
            if (Grid.instance.AreNeighbours(_firstCell,_secondCell))
            {
                OnValidMove();
            }
        }
    }

    private void OnValidMove()
    {
        MatchExecutionData matchExecutionData = new MatchExecutionData(new List<List<Element>>(),new List<GridCell>(),swipeNumber,_firstCell,_secondCell);
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
    }

   

    private bool IsFirstCellAssigned()
    {
        return _firstCell != null;
    }

  
}
