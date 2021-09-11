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
                Debug.Log("PERFORM SWIPE");
                
                SwapCells();
               // StartCoroutine(ReSwipeRoutine());
                
            }
        }
    }

    public void SwapCells()
    {
        Element firstElement = _firstCell.GetElement();
        Element secondElement = _secondCell.GetElement();
              
        _firstCell.SetElement(secondElement);
        _secondCell.SetElement(firstElement);
    }

    private IEnumerator ReSwipeRoutine()
    {
        Matcher.instance.StartChecking();
        
        while (Grid.instance.IsAnimating)
            yield return null;
        
        yield return new WaitForSeconds(.2f);
        
         if (!Matcher.instance.HasMatches)
         {
             SwapCells();
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
