using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridColoumnCollapser : MonoBehaviour
{

    private readonly new WaitForSeconds elementDispatchDelay = new WaitForSeconds(0.02f);

    [ContextMenu("Test")]
    public void CollapseColomuns(MatchExecutionData executionData) {

        #region FUNCTION_EXECUTION_ORDER
        Grid grid = Grid.instance;


        List<ElementFromToPair> elementFromToPairs = new List<ElementFromToPair>();
        List<ElementFromToPair> elementFromToPairPyramid = new List<ElementFromToPair>();

        int[] dirtyColoumns = FastConvertorUtils.FastHashSetToArray(executionData.dirtyColoumns);
        System.Array.Sort(dirtyColoumns);

        ShiftCellDownwards(dirtyColoumns);
        ShiftInPyramid();
        AnimateMovement();
        #endregion


        #region LOCAL_FUNCTIONS


        void ShiftCellDownwards(int[] coloumns)
        {
            #region EXECUTION_ORDER
            for (int colId = coloumns.Length - 1; colId >= 0; colId--)
            {

                int coloumnIdx = coloumns[colId];
                List<int[]> fromToIndexPairs = new List<int[]>();

                fromToIndexPairs.Add(new int[2]);
                fromToIndexPairs[fromToIndexPairs.Count - 1][0] = 0;
                fromToIndexPairs[fromToIndexPairs.Count - 1][1] = 0;

                for (int k = 0; k < grid.GridHeight; k++)
                {
                    if (grid[k, coloumnIdx] == null || !grid[k, coloumnIdx].IsBlocked)
                    {
                        fromToIndexPairs[fromToIndexPairs.Count - 1][1] = k;
                    }
                    else if (grid[k, coloumnIdx].IsBlocked && k < grid.GridHeight - 1)
                    {
                        fromToIndexPairs.Add(new int[2]);
                        fromToIndexPairs[fromToIndexPairs.Count - 1][0] = k + 1;
                        fromToIndexPairs[fromToIndexPairs.Count - 1][1] = k + 1;
                    }

                }

                for (int k = 0; k < fromToIndexPairs.Count; k++)
                {
                    int start = fromToIndexPairs[k][0];
                    int endt = fromToIndexPairs[k][1];


                    if (start == endt && grid[start, coloumnIdx] != null)
                    {
                        grid[start, coloumnIdx].renderer.color = Color.magenta;
                        continue;
                    }

                    Color color = Random.ColorHSV();

                    for (int j = endt; j >= start; j--)
                    {

                        GridCell currentCell = grid[j, coloumnIdx];

                        if (currentCell == null)
                            continue;

                        //currentCell.renderer.color = color;
                        if (currentCell.IsEmpty)
                        {
                            GridCell filledCell = GetTopMostFilledCell(coloumnIdx, start, j);

                            if (filledCell == null)
                            {
                                break;
                            }

                            Element element = filledCell.GetElement();
                            currentCell.SetElement(element);
                            elementFromToPairs.Add(new ElementFromToPair(element, filledCell, currentCell));
                        }
                    }
                }
            }
            #endregion

            #region LOCAL_FUNCTIONS
            GridCell GetTopMostFilledCell(int coloumnIdx, int st, int ed)
            {

                for (int tpI = ed; tpI >= st; tpI--)
                {
                    GridCell currentCell = grid[tpI, coloumnIdx];
                    if (currentCell != null && !currentCell.IsEmpty)
                    {
                        return currentCell;
                    }
                }
                return null;

            }
            #endregion
        }

        void ShiftInPyramid()
        {
            int whileSafeCheck = 0;
            bool infLooperror=false;
            #region EXECUTION_ORDER
            for (int gI = grid.GridHeight - 1; gI >= 0; gI--)
            {
                for (int gJ = grid.GridWidth - 1; gJ >= 0; gJ--)
                {

                    GridCell currentCell = grid[gI, gJ];
                    if (currentCell && !currentCell.IsEmpty && !currentCell.IsBlocked)
                    {

                        GridCell bottomFromCurrent;
                        GridCell bottomRightFromCurrent;
                        GridCell bottomLeftFromCurrent;

                        do
                        {
                            RefreshBRL(out bottomFromCurrent, out bottomRightFromCurrent, out bottomLeftFromCurrent, gI, gJ);

                            string log = string.Empty;

                            log += $"Current cell {currentCell} \n";
                            log += ($"Bottom cell {bottomFromCurrent} is null {bottomFromCurrent == null} \n");
                            log += ($"BottomLeft cell {bottomLeftFromCurrent} is null {bottomLeftFromCurrent == null} \n");
                            log += ($"BottomRight cell {bottomRightFromCurrent} is null {bottomRightFromCurrent == null} \n");

                            Debug.Log(log);

                            if (bottomFromCurrent != null)
                            {
                                gI++;
                                Element element = currentCell.GetElement();
                                bottomFromCurrent.SetElement(element);
                                elementFromToPairPyramid.Add(new ElementFromToPair(element, currentCell, bottomFromCurrent));
                                continue;
                            }

                            if (bottomRightFromCurrent != null)
                            {
                                gI++;
                                gJ++;
                                Element element = currentCell.GetElement();
                                bottomRightFromCurrent.SetElement(element);
                                elementFromToPairPyramid.Add(new ElementFromToPair(element, currentCell, bottomRightFromCurrent));
                                continue;
                            }

                            if (bottomLeftFromCurrent != null)
                            {

                                gI++;
                                gJ--;
                                Element element = currentCell.GetElement();
                                bottomLeftFromCurrent.SetElement(element);
                                elementFromToPairPyramid.Add(new ElementFromToPair(element, currentCell, bottomLeftFromCurrent));
                                continue;

                            }

                            whileSafeCheck++;

                            if (whileSafeCheck >= 150) {
                                infLooperror = true;
                                whileSafeCheck = 0;
                                break;
                            }
                        } while (bottomFromCurrent != null || bottomRightFromCurrent != null || bottomLeftFromCurrent != null);

                    }

                }
            }

            if (infLooperror) {
                Debug.LogError("INFINITE LOOP ENCOUNTERED !");
            }
            #endregion

            #region LOCAL_FUNCTIONS
            void RefreshBRL(out GridCell bottom,out GridCell bottomRight,out GridCell bottomLeft,int ti,int tj) {

                bottom = null;
                bottomRight = null;
                bottomLeft = null;

                //bottom
                int bottomI = ti + 1;
                int bottomJ = tj;

                if (bottomI < grid.GridHeight && grid[bottomI, bottomJ] && grid[bottomI, bottomJ].IsEmpty && !grid[bottomI, bottomJ].IsBlocked) {
                    bottom = grid[bottomI,bottomJ];
                }

                //bottomRight
                int bottomRightI = ti + 1;
                int bottomRightJ = tj + 1;


                bool isCellBelowObstacle = GameplayObstacleHandler.instance.IsCellBelowObstacle(bottomRightI,bottomRightJ);
                if (bottomRightI < grid.GridHeight && bottomRightJ < grid.GridWidth && 
                    grid[bottomRightI, bottomRightJ] && grid[bottomRightI, bottomRightJ].IsEmpty && 
                    !grid[bottomRightI, bottomRightJ].IsBlocked && isCellBelowObstacle) 
                {
                    bottomRight = grid[bottomRightI, bottomRightJ];
                }

                //bottomLeft
                int bottomLeftI = ti + 1;
                int bottomLeftJ = tj - 1;

                 isCellBelowObstacle = GameplayObstacleHandler.instance.IsCellBelowObstacle(bottomLeftI, bottomLeftJ);

                if (bottomLeftI < grid.GridHeight && bottomLeftJ >= 0 &&
                    grid[bottomLeftI, bottomLeftJ] && grid[bottomLeftI, bottomLeftJ].IsEmpty &&
                    !grid[bottomLeftI, bottomLeftJ].IsBlocked && isCellBelowObstacle)
                {
                    bottomLeft = grid[bottomLeftI, bottomLeftJ];
                }

            }

           
            #endregion

        }


        void AnimateMovement() {

            StartCoroutine(AnimateMovementRoutine());
        }

        IEnumerator AnimateMovementRoutine() {

            

            //Debug.LogError("ANIM START");
            for (int i = 0; i < elementFromToPairs.Count; i++)
            {
                ElementFromToPair movementPair = elementFromToPairs[i];
                Element element = movementPair.Element;
                GridCell toCell = movementPair.ToCell;
                element.AnimateToCell(toCell);
                yield return elementDispatchDelay;
            }

            for (int i = 0; i < elementFromToPairPyramid.Count; i++)
            {
                ElementFromToPair movementPair = elementFromToPairPyramid[i];
                Element element = movementPair.Element;
                GridCell toCell = movementPair.ToCell;

                if(element == null)
                    Debug.Log($"element null -> {movementPair.elementName} fromCell -> {movementPair.FromCell} toCell -> {toCell}");

                if (element)
                {
                    MatchExecutionData executionDataFromCell = movementPair.FromCell.executionData;

                    if (movementPair.ToCell.executionData==null || !movementPair.ToCell.executionData.Equals(executionDataFromCell))
                        movementPair.ToCell.SetExecutionData(executionDataFromCell);
                
                    element.AnimateToCell(toCell);
                }
                yield return elementDispatchDelay;
            }
            //Debug.LogError("ANIM END");

            yield return null;
        }
        #endregion

    }


    public struct ElementFromToPair
    {

        public Element Element;
        public GridCell FromCell;
        public GridCell ToCell;
        public string elementName;

        public ElementFromToPair(Element element, GridCell fromCell, GridCell toCell)
        {
            Element = element;
            FromCell = fromCell;
            ToCell = toCell;
            elementName = "";// element.gameObject.name;
        }
    }


}






