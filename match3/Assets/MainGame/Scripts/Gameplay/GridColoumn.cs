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
        SetNewlyGeneratedElementsToEmptyCells(null);
    }

    public void CollapseColoumn(MatchExecutionData executionData)
    {
        ShiftRemainingCellsToEmptySpaces(executionData);
      //  GenerateNewElementBuffer();
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
            gridCells[i].ToggleInputInteractibility(true);
            gridCells[i].ClearExecutionData();
            gridCells[i].renderer.color = Color.gray;
        }
    }

    #endregion

    #region PRIVATE_VARIABLES
    private void ShiftRemainingCellsToEmptySpaces(MatchExecutionData executionData)
    {
        //shift only downwards
        int max = GetMaxReachableCellInColoumn() - 1;
        


        cellIndex = max;
        for (int i = max; i >= 0; i--)
        {

            if (IsAdjacentCellAvailable(i, _coloumnToMyLeft)) {
                Debug.Log($"Adjacent cell available at my left{_coloumnToMyLeft.gameObject.name} and im{gameObject.name}");
            }



            Element element = gridCells[i].GetElement();
            if (element != null)
            {
                gridCells[cellIndex].SetElement(element);
                cellIndex--;
            }
        }


    }

    private bool IsAdjacentCellAvailable(int currentIndex,GridColoumn otherColoumn) {

        if (otherColoumn == null)
            return false;

        if (!otherColoumn.IsBlockedByObstacle())
            return false;

        int adjacentIndex = currentIndex + 1;
        if (adjacentIndex >= otherColoumn.ColoumnLength)
            return false;

        GridCell adjacentCell = otherColoumn[adjacentIndex];

        if (adjacentCell.IsBlocked || !adjacentCell.IsEmpty)
            return false;

        return true;
    }


    private int ExtraCellsRequired()
    {
        int cellsRequired = 0;
        bool hasBlockades = false;
        for (int i = 0; i < gridCells.Count; i++)
        {
            if (gridCells[i].IsBlocked && !hasBlockades)
            {
                hasBlockades = true;
            }

            if (hasBlockades)
            {
                cellsRequired++;
            }
        }

        return cellsRequired;
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
