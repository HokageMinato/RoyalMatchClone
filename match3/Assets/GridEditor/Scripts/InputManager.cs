using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class InputManager : Singleton<InputManager>
{
    private GridCell _firstCell;
    private GridCell _secondCell;
    private bool _inputValid = false;

    public void SetFirstCell(GridCell firstCell)
    {
        _firstCell = firstCell;
        ValidateInputsForSwipe();
    }

   

    public void SetSecondCell(GridCell secondCell)
    {
        _secondCell = secondCell;
        ValidateMove();
        InvalidateNextInputsTillFingerLift();
    }

    private void InvalidateNextInputsTillFingerLift()
    {
        _inputValid = false;
    }

    private void ValidateMove()
    {
        if (IsFirstCellAssigned() && _inputValid)
        {
            if (_secondCell.IsNeighbourOf(_firstCell))
            {
                Debug.Log("PERFORM SWIPE");
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
