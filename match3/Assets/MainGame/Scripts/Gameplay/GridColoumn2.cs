//using System.Collections.Generic;
//using UnityEngine;

//public class GridColoumn2 : MonoBehaviour
//{
//    #region PRIVATE_VARIABLES
//    private ElementFactory elementGenerator;
//    private List<GridCell> gridCells = new List<GridCell>();
//    private List<Element> _generatedElementList = new List<Element>();
//    private int cellIndex = 0;
//    #endregion

//    #region PUBLIC_METHODS
//    public void Init(ElementFactory elementGenerat)
//    {
//        elementGenerator = elementGenerat;
//        elementGenerator.transform.SetParent(transform);
//        elementGenerator.transform.localPosition = Vector3.up * 1.5f;
//        cellIndex = GetMaxReachableCellInColoumn() - 1;
//        GenerateNewElementBuffer();
//        SetNewlyGeneratedElementsToEmptyCells(null);
//    }

//    public void CollapseColoumn(MatchExecutionData executionData)
//    {
//        ShiftRemainingCellsToEmptySpaces(executionData);
//        GenerateNewElementBuffer();
//        SetNewlyGeneratedElementsToEmptyCells(executionData);
//    }
//    public void AddCell(GridCell newCell)
//    {
//        gridCells.Add(newCell);
//    }

//    public void LockColoumn(MatchExecutionData executionData)
//    {
//        for (int i = 0; i < gridCells.Count; i++)
//        {
//            gridCells[i].renderer.color = Color.red;
//            gridCells[i].SetExecutionData(executionData);
//            gridCells[i].ToggleInputInteractibility(false);
//        }
//    }

//    public void UnLockColoumn()
//    {
//        for (int i = 0; i < gridCells.Count; i++)
//        {
//            gridCells[i].ToggleInputInteractibility(true);
//            gridCells[i].ClearExecutionData();
//            gridCells[i].renderer.color = Color.gray;
//        }
//    }

//    #endregion

//    #region PRIVATE_VARIABLES
//    private void ShiftRemainingCellsToEmptySpaces(MatchExecutionData executionData)
//    {
//        int max = GetMaxReachableCellInColoumn() - 1;
//        cellIndex = max;
//        for (int i = max; i >= 0; i--)
//        {
//            Element element = gridCells[i].GetElement();
//            if (element != null)
//            {
//                gridCells[cellIndex].SetElement(element);
//                cellIndex--;
//            }
//        }
//    }

//    private void GenerateNewElementBuffer()
//    {
//        _generatedElementList.Clear();
//        int max = GetMaxReachableCellInColoumn();
//        for (int i = 0; i < max; i++)
//        {
//            GridCell cell = gridCells[i];

//            if (cell.IsEmpty)
//            {
//                //Element element = elementGenerator.GetRandomElement(cell);
//                //Transform elementTransform = element.transform;
//                //Vector3 initialPosition = elementTransform.InverseTransformPoint(transform.position);
//                //initialPosition.y += i;
//                //elementTransform.localPosition = initialPosition;

//                //_generatedElementList.Add(element);
//            }
//        }
//    }

//    private void SetNewlyGeneratedElementsToEmptyCells(MatchExecutionData executionData)
//    {
//        for (int i = 0; i < _generatedElementList.Count; i++)
//        {
//            GridCell cell = gridCells[cellIndex];
//            cell.SetExecutionData(executionData);

//            Element element = _generatedElementList[i];
//            cell.SetElement(element);
//            cellIndex--;
//        }

//    }


//    private int GetMaxReachableCellInColoumn()
//    {
//        int maxRechable = gridCells.Count;
//        for (int i = 0; i < gridCells.Count; i++)
//        {
//            GridCell cell = gridCells[i];
//            if (cell.IsBlocked)
//            {
//                return i;
//            }
//        }
//        return maxRechable;
//    }
//    #endregion


//}
