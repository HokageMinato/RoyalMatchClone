using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : Singleton<InputManager>
{
    [SerializeField] private GridCell _firstCell;
    [SerializeField] private GridCell _secondCell;
    private bool _inputValid = false;

    public void SetFirstCell(GridCell firstCell)
    {
        _firstCell = firstCell;
        ValidateInputsForSwipe();
    }

   

    public void SetSecondCell(GridCell secondCell)
    {
        if (!_inputValid)
            return;
        
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
                Debug.Log("PERFORM SWIPE");
                Element firstElement = _firstCell.element;
                Element secondElement = _secondCell.element;
                
                firstElement.SetHolder(_secondCell);
                secondElement.SetHolder(_firstCell);

            }
        }
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
