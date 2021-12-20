using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridColoumnCollapser : MonoBehaviour
{
    public Transform transformActivePrefab;
    private readonly WaitForSeconds interAnimationChainDispatchDelay = new WaitForSeconds(0.1f);
    private Transform[] elementSpawnPositions;

    public void Init()
    {
        SetElementFactoryTransforms();
        GenerateInitialElements();
    }

    private void SetElementFactoryTransforms()
    {
        GridDesignTemp gridLevel = GameplayManager.instance.levelData;
        Grid grid = Grid.instance;
        int gridWidth = gridLevel.gridWidth;

        elementSpawnPositions = new Transform[gridWidth];
        for (int i = 0; i < elementSpawnPositions.Length; i++)
        {
            int cellType = gridLevel[0, i];

            if (cellType == GridConstants.NO_CELL ||
               ObstacleFactory.instance.IsBlockerType(cellType))
                continue;

            Transform parentTransform = grid.GetLayerTransformParent(RenderLayer.ElementLayer);
            elementSpawnPositions[i] = Instantiate(transformActivePrefab, parentTransform);
            elementSpawnPositions[i].transform.position = grid[0, i].transform.position + Vector3.up * GridDesignTemp.gridSpacing;
        }
    }

    private void GenerateInitialElements()
    {
        Grid grid = Grid.instance;

        for (int i = 0; i < grid.GridHeight; i++)
        {
            for (int j = 0; j < grid.GridWidth; j++)
            {

                GridCell gridCell = grid[i, j];
                if (!gridCell || gridCell.IsBlocked)
                    continue;


                Element newElement = GenerateElementAt(gridCell);
                newElement.transform.position = gridCell.transform.position;
            }
        }

    }

    private Element GenerateElementAt(GridCell cell)
    {

        Element element = ElementFactory.instance.GenerateRandomElement();
        cell.SetElement(element);
        return element;
    }

    public void CollapseColomuns(MatchExecutionData executionData)
    {

        #region FUNCTION_EXECUTION_ORDER
        Grid grid = Grid.instance;

        Dictionary<Element, List<ElementAnimationData>> elementFromToPairForAnimation = new Dictionary<Element, List<ElementAnimationData>>();
        int iterationPassRequiredForZigZagPaths = grid.GridHeight;
        for (int i = 0; i < iterationPassRequiredForZigZagPaths; i++)
        {

            ShiftCellsDown();

            ShiftCellsRightAndDown();

            ShiftCellsLeftAndDown();

            GenerateNewElements();

        }


        AnimateMovement();
        #endregion

        #region LOCAL_FUNCTION_DECLARATIONS
        void AddPair(Element element, ElementAnimationData animationData)
        {

            if (!elementFromToPairForAnimation.ContainsKey(element))
                elementFromToPairForAnimation.Add(element, new List<ElementAnimationData>());

            elementFromToPairForAnimation[element].Add(animationData);
        }

        void ShiftCellsDown()
        {
            int whileSafeCheck = 0;
            bool infLooperror = false;

            for (int bI = grid.GridHeight - 1; bI >= 0; bI--)
            {
                for (int bJ = grid.GridWidth - 1; bJ >= 0; bJ--)
                {

                    GridCell currentCell = grid[bI, bJ];
                    if (currentCell && !currentCell.IsEmpty && !currentCell.IsBlocked)
                    {

                        GridCell bottomFromCurrent;
                        do
                        {
                            RefreshB(out bottomFromCurrent, bI, bJ);

                            string log = string.Empty;
                            log += $"Current cell {currentCell} \n";
                            log += ($"Bottom cell {bottomFromCurrent} is null {bottomFromCurrent == null} \n");
                            //   Debug.Log(log);


                            if (bottomFromCurrent != null)
                            {
                                bI++;
                                Element element = currentCell.GetElement();
                                bottomFromCurrent.SetElement(element);
                                currentCell = bottomFromCurrent;

                                //elementFromToPairForAnimation.Add(new ElementAnimationData(element, currentCell, bottomFromCurrent));
                                AddPair(element, new ElementAnimationData(element, currentCell, bottomFromCurrent));
                                continue;
                            }



                            whileSafeCheck++;

                            if (whileSafeCheck >= 350)
                            {
                                infLooperror = true;
                                whileSafeCheck = 0;
                                break;
                            }
                        } while (bottomFromCurrent != null);

                    }

                }
            }

            void RefreshB(out GridCell bottom, int ti, int tj)
            {

                bottom = null;

                //bottom
                int bottomI = ti + 1;
                int bottomJ = tj;

                if (bottomI < grid.GridHeight && grid[bottomI, bottomJ] && grid[bottomI, bottomJ].IsEmpty && !grid[bottomI, bottomJ].IsBlocked)
                {
                    bottom = grid[bottomI, bottomJ];
                }

            }

            if (infLooperror)
            {
                Debug.LogError("INFINITE LOOP ENCOUNTERED BOTTOM!");
            }
        }

        void ShiftCellsRightAndDown()
        {

            int whileSafeCheck = 0;
            bool infLooperror = false;

            for (int gI = grid.GridHeight - 1; gI >= 0; gI--)
            {
                for (int gJ = grid.GridWidth - 1; gJ >= 0; gJ--)
                {

                    GridCell currentCell = grid[gI, gJ];
                    if (currentCell && !currentCell.IsEmpty && !currentCell.IsBlocked)
                    {
                        GridCell bottomFromCurrent;
                        GridCell bottomRightFromCurrent;

                        do
                        {
                            RefreshBR(out bottomFromCurrent, out bottomRightFromCurrent, gI, gJ);

                            //string log = string.Empty;

                            //log += $"Current cell {currentCell} \n";
                            //log += ($"Bottom cell {bottomFromCurrent} is null {bottomFromCurrent == null} \n");
                            //log += ($"BottomRight cell {bottomRightFromCurrent} is null {bottomRightFromCurrent == null} \n");

                            //   Debug.Log(log);

                            if (bottomFromCurrent != null)
                            {
                                gI++;
                                Element element = currentCell.GetElement();
                                bottomFromCurrent.SetElement(element);
                                currentCell = bottomFromCurrent;
                                //elementFromToPairForAnimation.Add(new ElementAnimationData(element, currentCell, bottomFromCurrent));
                                AddPair(element, new ElementAnimationData(element, currentCell, bottomFromCurrent));
                                continue;
                            }

                            if (bottomRightFromCurrent != null)
                            {
                                gI++;
                                gJ++;
                                Element element = currentCell.GetElement();
                                bottomRightFromCurrent.SetElement(element);
                                currentCell = bottomRightFromCurrent;
                                //elementFromToPairForAnimation.Add(new ElementAnimationData(element, currentCell, bottomRightFromCurrent));
                                AddPair(element, new ElementAnimationData(element, currentCell, bottomRightFromCurrent));
                                continue;
                            }


                            whileSafeCheck++;

                            if (whileSafeCheck >= 150)
                            {
                                infLooperror = true;
                                whileSafeCheck = 0;
                                break;
                            }
                        } while (bottomFromCurrent != null || bottomRightFromCurrent != null);

                        if (infLooperror)
                        {
                            Debug.LogError("INFINITE LOOP ENCOUNTERED RIGHT!");
                        }

                    }
                }
            }


            void RefreshBR(out GridCell bottom, out GridCell bottomRight, int ti, int tj)
            {

                bottom = null;
                bottomRight = null;

                //bottom
                int bottomI = ti + 1;
                int bottomJ = tj;

                if (bottomI < grid.GridHeight && grid[bottomI, bottomJ] && grid[bottomI, bottomJ].IsEmpty && !grid[bottomI, bottomJ].IsBlocked)
                {
                    bottom = grid[bottomI, bottomJ];
                }

                //bottomRight
                int bottomRightI = ti + 1;
                int bottomRightJ = tj + 1;


                bool isCellBelowObstacle = GameplayObstacleHandler.instance.IsCellBelowObstacle(bottomRightI, bottomRightJ);
                if (bottomRightI < grid.GridHeight && bottomRightJ < grid.GridWidth &&
                    grid[bottomRightI, bottomRightJ] && grid[bottomRightI, bottomRightJ].IsEmpty &&
                    !grid[bottomRightI, bottomRightJ].IsBlocked && isCellBelowObstacle)
                {
                    bottomRight = grid[bottomRightI, bottomRightJ];
                }
            }
        }

        void ShiftCellsLeftAndDown()
        {

            int whileSafeCheck = 0;
            bool infLooperror = false;


            for (int gI = grid.GridHeight - 1; gI >= 0; gI--)
            {
                for (int gJ = grid.GridWidth - 1; gJ >= 0; gJ--)
                {

                    GridCell currentCell = grid[gI, gJ];
                    if (currentCell && !currentCell.IsEmpty && !currentCell.IsBlocked)
                    {

                        GridCell bottomFromCurrent;
                        GridCell bottomLeftFromCurrent;

                        do
                        {
                            RefreshBL(out bottomFromCurrent, out bottomLeftFromCurrent, gI, gJ);

                            //string log = string.Empty;

                            //log += $"Current cell {currentCell} \n";
                            //log += ($"Bottom cell {bottomFromCurrent} is null {bottomFromCurrent == null} \n");
                            //log += ($"BottomLeft cell {bottomLeftFromCurrent} is null {bottomLeftFromCurrent == null} \n");

                            //  Debug.Log(log);

                            if (bottomFromCurrent != null)
                            {
                                gI++;
                                Element element = currentCell.GetElement();
                                bottomFromCurrent.SetElement(element);
                                currentCell = bottomFromCurrent;
                                //elementFromToPairForAnimation.Add(new ElementAnimationData(element, currentCell, bottomFromCurrent));
                                AddPair(element, new ElementAnimationData(element, currentCell, bottomFromCurrent));
                                continue;
                            }

                            if (bottomLeftFromCurrent != null)
                            {

                                gI++;
                                gJ--;
                                Element element = currentCell.GetElement();
                                bottomLeftFromCurrent.SetElement(element);
                                currentCell = bottomLeftFromCurrent;
                                //elementFromToPairForAnimation.Add(new ElementAnimationData(element, currentCell, bottomLeftFromCurrent));
                                AddPair(element, new ElementAnimationData(element, currentCell, bottomLeftFromCurrent));
                                continue;

                            }

                            whileSafeCheck++;

                            if (whileSafeCheck >= 150)
                            {
                                infLooperror = true;
                                whileSafeCheck = 0;
                                break;
                            }
                        } while (bottomFromCurrent != null || bottomLeftFromCurrent != null);

                        if (infLooperror)
                        {
                            Debug.LogError("INFINITE LOOP ENCOUNTERED RIGHT!");
                        }

                    }

                }
            }



            void RefreshBL(out GridCell bottom, out GridCell bottomLeft, int ti, int tj)
            {

                bottom = null;
                bottomLeft = null;

                //bottom
                int bottomI = ti + 1;
                int bottomJ = tj;

                if (bottomI < grid.GridHeight && grid[bottomI, bottomJ] && grid[bottomI, bottomJ].IsEmpty && !grid[bottomI, bottomJ].IsBlocked)
                {
                    bottom = grid[bottomI, bottomJ];
                }


                //bottomLeft
                int bottomLeftI = ti + 1;
                int bottomLeftJ = tj - 1;

                bool isCellBelowObstacle = GameplayObstacleHandler.instance.IsCellBelowObstacle(bottomLeftI, bottomLeftJ);

                if (bottomLeftI < grid.GridHeight && bottomLeftJ >= 0 &&
                    grid[bottomLeftI, bottomLeftJ] && grid[bottomLeftI, bottomLeftJ].IsEmpty &&
                    !grid[bottomLeftI, bottomLeftJ].IsBlocked && isCellBelowObstacle)
                {
                    bottomLeft = grid[bottomLeftI, bottomLeftJ];
                }

            }




        }

        void GenerateNewElements()
        {

            for (int i = 0; i < grid.GridWidth; i++)
            {

                Vector3 position = grid[0, i].transform.position;
                for (int j = 0; j < grid.GridHeight; j++)
                {
                    GridCell currentCell = grid[j, i];

                    if (currentCell == null)
                        continue;

                    if (!currentCell.IsEmpty)
                        break;

                    position.y += GridDesignTemp.gridSpacing;
                    Element newElement = GenerateElementAt(currentCell);
                    newElement.transform.position = position;
                    //elementFromToPairForAnimation.Add(new ElementAnimationData(newElement, currentCell, currentCell));
                    AddPair(newElement, new ElementAnimationData(newElement, currentCell, currentCell));
                }
            }
        }

        void AnimateMovement()
        {

            StartCoroutine(AnimateMovementRoutine());
        }

        IEnumerator AnimateMovementRoutine()
        {


            foreach (KeyValuePair<Element, List<ElementAnimationData>> pair in elementFromToPairForAnimation)
            {
                yield return StartCoroutine(AnimateElementChain(pair.Value));
            }



            yield return null;
        }

        IEnumerator AnimateElementChain(List<ElementAnimationData> elementAnimationDatas)
        {


            //Reverse sort by ToIndex and if similar reverseSort that set by FromIndex to get dependency chain.

            for (int i = 0; i < elementAnimationDatas.Count; i++)
            {
                ElementAnimationData animationData = elementAnimationDatas[i];
                GridCell toCell = animationData.ToCell;
                Element element = animationData.Element;

                //TEMPORARY FIX 
                /// ISSUE DUE TO DECLUSION OF BLOCKED COLOUMNS DURING EXECUTIONDATA's LOCKED COLOUMN ASSIGING
                /// ADD NEARBY BLOCKED COLOUMNS WHILE LOCKING AND ASSIGN APPROPRIATE EXEC DATA.
                if (toCell.executionData == null)
                {
                    toCell.SetExecutionData(executionData);
                }

                yield return element.AnimateToCellRoutine(toCell);
            }

            yield return null;
        }
    }
    #endregion

    #region LOCAL_DS_DEFINITIONS

    public struct ElementAnimationData
    {

        public Element Element;
        public GridCell FromCell;
        public GridCell ToCell;
        public string elementName;

        public ElementAnimationData(Element element, GridCell fromCell, GridCell toCell)
        {
            Element = element;
            FromCell = fromCell;
            ToCell = toCell;
            elementName = element.gameObject.name;

        }
    }

    #endregion
}