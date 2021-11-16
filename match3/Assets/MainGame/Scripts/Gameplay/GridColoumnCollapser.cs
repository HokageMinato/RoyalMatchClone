using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridColoumnCollapser : MonoBehaviour
{

    private readonly new WaitForSeconds elementDispatchDelay = new WaitForSeconds(0.02f);

    [ContextMenu("Test")]
    public void CollapseColomuns(MatchExecutionData executionData) {

        #region FUNCTION_EXECUTION_ORDER
        //use patterncells's wIndex to obtain locked coloumns' executionData;
        
        Grid grid = Grid.instance;
        List<ElementFromToPair> elementFromToPairs = new List<ElementFromToPair>();
        int[] dirtyColoumns = FastConvertorUtils.FastHashSetToArray(executionData.dirtyColoumns);
        System.Array.Sort(dirtyColoumns);
        
        ShiftCellDownwards(dirtyColoumns);
        ShiftInPyramid();
        

        


        AnimateMovement();
        #endregion


        #region LOCAL_FUNCTIONS

        
        void ShiftCellDownwards(int[] coloumns)
        {
            for (int colId = coloumns.Length- 1; colId >= 0; colId--)
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
                            GridCell filledCell = GetTopMostFilledCell(coloumnIdx,start, j);

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


            GridCell GetTopMostFilledCell(int coloumnIdx,int st, int ed)
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
        }

        void ShiftInPyramid() { 
            
        }




        
        void AnimateMovement() {

            StartCoroutine(AnimateMovementRoutine());
        }

        IEnumerator AnimateMovementRoutine() {
            for (int i = 0; i < elementFromToPairs.Count; i++)
            {
                ElementFromToPair movementPair = elementFromToPairs[i];
                Element element = movementPair.Element;
                GridCell toCell = movementPair.ToCell;
                
                element.AnimateToCell(toCell);

                yield return elementDispatchDelay;
            }
            yield return null;
        }
        #endregion

    }



    public struct ElementFromToPair {

        public Element Element;
        public GridCell FromCell;
        public GridCell ToCell;

        public ElementFromToPair(Element element, GridCell fromCell, GridCell toCell) {
            Element = element;
            FromCell = fromCell;
            ToCell = toCell;
        }
    }




}
