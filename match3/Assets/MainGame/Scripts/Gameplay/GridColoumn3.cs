//using System.Collections.Generic;
//using UnityEngine;

//public class GridColoumn3 : MonoBehaviour
//{
//    #region PRIVATE_VARIABLES
//    private ElementFactory _elementFactory;
//    private List<GridCell> _gridCells = new List<GridCell>();
//    private List<Element> _generatedElementList = new List<Element>();
//    private GridColoumn _coloumnToMyLeft;
//    private GridColoumn _coloumnToMyRight;

//    public int ColoumnLength
//    {
//        get
//        {
//            return _gridCells.Count;
//        }
//    }
//    #endregion

//    #region PUBLIC_PROPERTIES
//    public GridCell this[int index]
//    {
//        get
//        {
//            return _gridCells[index];
//        }
//    }
//    #endregion

//    #region PUBLIC_METHODS
//    public void Init(ElementFactory elementFactory, GridColoumn leftColoumn, GridColoumn rightColoumn)
//    {
//        _elementFactory = elementFactory;
//        _coloumnToMyLeft = leftColoumn;
//        _coloumnToMyRight = rightColoumn;
//        _elementFactory.transform.SetParent(transform);
//        _elementFactory.transform.localPosition = Vector3.zero;
//        GenerateElements();
//    }


//    public void ShiftRemainingCellsDownwards(MatchExecutionData executionData)
//    {
//        List<int[]> fromToIndexPairs = new List<int[]>();

//        fromToIndexPairs.Add(new int[2]);
//        fromToIndexPairs[fromToIndexPairs.Count - 1][0] = 0;
//        fromToIndexPairs[fromToIndexPairs.Count - 1][1] = 0;

//        for (int i = 0; i < _gridCells.Count; i++)
//        {
//            if (_gridCells[i] == null || !_gridCells[i].IsBlocked)
//            {
//                fromToIndexPairs[fromToIndexPairs.Count - 1][1] = i;
//            }
//            else if (_gridCells[i].IsBlocked && i < _gridCells.Count - 1)
//            {
//                fromToIndexPairs.Add(new int[2]);
//                fromToIndexPairs[fromToIndexPairs.Count - 1][0] = i + 1;
//                fromToIndexPairs[fromToIndexPairs.Count - 1][1] = i + 1;
//            }

//        }

//        for (int i = 0; i < fromToIndexPairs.Count; i++)
//        {
//            int start = fromToIndexPairs[i][0];
//            int endt = fromToIndexPairs[i][1];


//            if (start == endt)
//            {
//                _gridCells[start].renderer.color = Color.red;
//                continue;
//            }

//            Color color = Random.ColorHSV();

//            for (int j = endt; j >= start; j--)
//            {
//                _gridCells[j].renderer.color = color;

//                GridCell currentCell = _gridCells[j];
//                if (currentCell.IsEmpty)
//                {
//                    GridCell filledCell = GetTopMostFilledCell(start, j);

//                    if (filledCell == null)
//                    {
//                        break;
//                    }
//                    currentCell.SetElement(filledCell.GetElement());
//                }
//            }
//        }

//        GridCell GetTopMostFilledCell(int st, int ed)
//        {

//            for (int i = ed; i >= st; i--)
//            {
//                if (!_gridCells[i].IsEmpty)
//                {
//                    return _gridCells[i];
//                }
//            }
//            return null;

//        }


//    }

//    public void ShiftRemainingCellsPyramid(MatchExecutionData executionData)
//    {



//    }

//    public void AddCell(GridCell newCell)
//    {
//        _gridCells.Add(newCell);
//    }

//    public void LockColoumn(MatchExecutionData executionData)
//    {
//        for (int i = 0; i < _gridCells.Count; i++)
//        {
//            _gridCells[i].renderer.color = Color.red;
//            _gridCells[i].SetExecutionData(executionData);
//            _gridCells[i].ToggleInputInteractibility(false);
//        }
//    }

//    public void UnLockColoumn()
//    {
//        for (int i = 0; i < _gridCells.Count; i++)
//        {
//            _gridCells[i].ToggleInputInteractibility(true);
//            _gridCells[i].SetExecutionData(null);
//            _gridCells[i].renderer.color = Color.gray;
//        }
//    }

//    public override string ToString()
//    {
//        return gameObject.name;
//    }
//    #endregion

//    #region PRIVATE_METHODS

//    private GridCell[] CreateCellPairList(int index)
//    {
//        const int leftIndex = 1;
//        const int rightIndex = 2;
//        const int middleIndex = 0;

//        GridCell[] cellPair = new GridCell[3];

//        //cellPair[middleIndex] = SearchForEmptyCellFromBottomTill(index);

//        //if (_coloumnToMyLeft != null && _coloumnToMyLeft.IsBlockedByObstacle())
//        //    cellPair[leftIndex] = _coloumnToMyLeft.SearchForEmptyCellFromBottomTill(index);

//        //if (_coloumnToMyRight != null && _coloumnToMyRight.IsBlockedByObstacle())
//        //    cellPair[rightIndex] = _coloumnToMyRight.SearchForEmptyCellFromBottomTill(index);

//        return cellPair;
//    }


//    private bool IsBlockedByObstacle()
//    {

//        for (int i = 0; i < _gridCells.Count; i++)
//        {
//            if (_gridCells[i].IsBlocked)
//                return true;
//        }
//        return false;
//    }




//    private void GenerateElements()
//    {

//        for (int i = 0; i < _gridCells.Count; i++)
//        {
//            if (_gridCells[i].IsBlocked)
//                continue;

//            Element randomElement = _elementFactory.GetRandomElement();
//            randomElement.transform.position = _gridCells[i].transform.position;
//            _gridCells[i].SetElement(randomElement);
//        }
//    }

//    #endregion



//}


///*
// CAller code in grid

//public void CollapseColoumns(MatchExecutionData executionData)
//    {
        
//        for (int l = _gridC.Count-1; l >=0; l--)
//        {
//            _gridC[l].ShiftRemainingCellsDownwards(executionData);
//        }
        
//        for (int l = _gridC.Count-1; l >=0; l--)
//        {
//            _gridC[l].ShiftRemainingCellsPyramid(executionData);
//        }


        
//    }
 
// */