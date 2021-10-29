using System.Collections.Generic;
using UnityEngine;

public class GridColoumn : MonoBehaviour
{
    #region PRIVATE_VARIABLES
    private ElementGenerator elementGenerator;
    private List<GridCell> gridCells = new List<GridCell>();
    private List<Element> _generatedElementList = new List<Element>();
    private int cellIndex = 0;
    private GridColoumn _coloumnToMyLeft;
    private GridColoumn _coloumnToMyRight;

    public int ColoumnLength
    {
        get {
            return gridCells.Count;
        }
    }
    #endregion

    #region PUBLIC_PROPERTIES
    public GridCell this[int index]
    {
        get {
            return gridCells[index];
        }
    }
    #endregion

    #region PUBLIC_METHODS
    public void Init(ElementGenerator elementGenerat,GridColoumn leftColoumn,GridColoumn rightColoumn)
    {
        elementGenerator = elementGenerat;
        _coloumnToMyLeft = leftColoumn;
        _coloumnToMyRight = rightColoumn;
        elementGenerator.transform.SetParent(transform);
        elementGenerator.transform.localPosition = Vector3.zero;
        cellIndex = GetMaxReachableCellInColoumn() - 1;
        GenerateNewElementBuffer();
        SetNewlyGeneratedElementsToEmptyCells(MatchExecutionData.GetDefaultExecutionData());
    }

    public void CollapseColoumn(MatchExecutionData executionData)
    {
      
        ShiftRemainingCellsToEmptySpaces(executionData);
        // GenerateNewElementBuffer();
        // SetNewlyGeneratedElementsToEmptyCells(executionData);
    }
    public void AddCell(GridCell newCell)
    {
        gridCells.Add(newCell);
    }

    public void LockColoumn(MatchExecutionData executionData)
    {
        for (int i = 0; i < gridCells.Count; i++)
        {
            gridCells[i].renderer.color = Color.red;
            gridCells[i].SetExecutionData(executionData);
            gridCells[i].ToggleInputInteractibility(false);
        }
    }

    public void UnLockColoumn()
    {
        for (int i = 0; i < gridCells.Count; i++)
        {
            Debug.Log("Unlocking");
            gridCells[i].ToggleInputInteractibility(true);
            gridCells[i].SetExecutionData(null);
            gridCells[i].renderer.color = Color.gray;
        }
    }

    #endregion

    #region PRIVATE_VARIABLES
   

    private void ShiftRemainingCellsToEmptySpaces(MatchExecutionData executionData)
    {
          int max = GetMaxReachableCellInColoumn() - 1;
            cellIndex = max;
            for (int i = max; i >= 0; i--)
            {
                Element element = gridCells[i].GetElement();
                if (element != null)
                {
                    gridCells[cellIndex].SetExecutionData(executionData);
                    gridCells[cellIndex].SetElement(element);
                    cellIndex--;
                }
            }
    }

    public int idxT;
    [ContextMenu("Testt")]
    public void Test() {
        GridCell[] cp = CreateCellPairList(idxT);
        for (int i = 0; i < cp.Length; i++)
        {
            if(cp[i]!=null)
            cp[i].renderer.color = Color.green;
        }

        
    }

    private GridCell[] CreateCellPairList(int index) {
        const int leftIndex = 1;
        const int rightIndex = 2;
        const int middleIndex = 0;

        GridCell[] cellPair = new GridCell[3];

        cellPair[middleIndex] = SearchForEmptyCellFromBottomTill(index);

        
        if (_coloumnToMyLeft != null)
            cellPair[leftIndex] = _coloumnToMyLeft.SearchForEmptyCellFromBottomTill(index);

        if (_coloumnToMyRight != null)
            cellPair[rightIndex] = _coloumnToMyRight.SearchForEmptyCellFromBottomTill(index);
    
        return cellPair;
    }


    



    public GridCell SearchForEmptyCellFromBottomTill(int currentIdx) {

        Debug.Log($"Searching from {ColoumnLength-1} to {currentIdx}");

        for (int i = ColoumnLength-1; i > currentIdx; i--)
        {
            if (this[i].IsEmpty && !this[i].IsBlocked)
                return this[i];

        }

        return null;
    }


    private bool IsBlockedByObstacle() {

        for (int i = 0; i < gridCells.Count; i++)
        {
            if (gridCells[i].IsBlocked)
                return true;
        }
        return false;
    }

    
    //-----------------------
    private void GenerateNewElementBuffer()
    {
        _generatedElementList.Clear();
        int max = GetMaxReachableCellInColoumn();
        Vector3 initialPosition;
        for (int i = 0; i < max; i++)
        {
            GridCell cell = gridCells[i];

            if (cell.IsEmpty)
            {
                Element element = elementGenerator.GetRandomElement();
                element.transform.SetParent(Grid.instance.transform);
                initialPosition = elementGenerator.transform.position;
                initialPosition.y += i;
                element.transform.position = initialPosition;

                _generatedElementList.Add(element);
            }
        }
    }

    private void SetNewlyGeneratedElementsToEmptyCells(MatchExecutionData executionData)
    {
        for (int i = 0; i < _generatedElementList.Count; i++)
        {
            GridCell cell = gridCells[cellIndex];
            cell.SetExecutionData(executionData);

            Element element = _generatedElementList[i];
            cell.SetElement(element);
            cellIndex--;
        }

    }

    private int GetMaxReachableCellInColoumn()
    {
        int maxRechable = gridCells.Count;
        for (int i = 0; i < gridCells.Count; i++)
        {
            GridCell cell = gridCells[i];
            if (cell.IsBlocked)
            {
                return i;
            }
        }
        return maxRechable;
    }
    #endregion


}


