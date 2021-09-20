﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : Singleton<InputManager>
{
    [SerializeField] private GridCell _firstCell;
    [SerializeField] private GridCell _secondCell;
    private bool _inputValid = false;
    private int swipeNumber = 0;

    public void SetFirstCell(GridCell firstCell)
    {
        if(firstCell.IsEmpty)
            return;
        
        _firstCell = firstCell;
        ValidateInputsForSwipe();
    }

   

    public void SetSecondCell(GridCell secondCell)
    {
        if (!_inputValid)
            return;

        if (secondCell.IsEmpty)
        {
            _inputValid = true;
            _firstCell = null;
            _secondCell = null;
        }

        InvalidateNextInputsTillFingerLift();
        _secondCell = secondCell;
        ValidateMove();
    }

    private void InvalidateNextInputsTillFingerLift()
    {
        _inputValid = false;
    }

    private void ValidateMove()           
    {
        if (IsFirstCellAssigned())
        {
            if (_secondCell.IsNeighbourOf(_firstCell))
            {
                OnValidMove();
            }
        }
    }

    private void OnValidMove()
    {
        Debug.Log("PERFORM SWIPE");
        MatchExecutionData matchExecutionData = new MatchExecutionData(new List<List<Element>>(),new List<GridCell>(),swipeNumber,_firstCell,_secondCell);
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

    private void ValidateInputsForSwipe()
    {
        _inputValid = true;
    }
    
    
}
