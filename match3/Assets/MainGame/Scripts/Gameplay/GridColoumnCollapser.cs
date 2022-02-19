using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GridColoumnCollapser : MonoBehaviour
{
    public Transform transformActivePrefab;
    
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
            elementSpawnPositions[i] = Instantiate(transformActivePrefab,parentTransform);
            elementSpawnPositions[i].transform.position = grid[0, i].transform.position + Vector3.up * GridDesignTemp.gridSpacing;
        }
    }

    private void GenerateInitialElements() {
        Grid grid = Grid.instance;

        for (int i = 0; i < grid.GridHeight; i++)
        {
            for (int j = 0; j < grid.GridWidth; j++) {

                GridCell gridCell = grid[i, j];
                if (!gridCell || gridCell.IsBlocked)
                    continue;


               Element newElement= GenerateElementAt(gridCell);
               newElement.transform.position = gridCell.transform.position; 
            }
        }
    
    }

    private Element GenerateElementAt(GridCell cell) {
        
        Element element = ElementFactory.instance.GenerateRandomElement();
        cell.SetElement(element);
        return element;
    }

    public void CollapseColomuns(MatchExecutionData executionData) 
    {

        #region FUNCTION_EXECUTION_ORDER
        Grid grid = Grid.instance;

        Dictionary<int, List<ElementAnimationData>> elementFromToPairForAnimation = new Dictionary<int, List<ElementAnimationData>>();
        

        int iterationPassRequiredForZigZagPaths = grid.GridHeight;

        for (int i = 0; i < iterationPassRequiredForZigZagPaths; i++)
        {
            ShiftCellsDown();

            ShiftCellsRightAndDown();

            ShiftCellsLeftAndDown();

        }
        
        AnimateMovement();


        #endregion

        #region LOCAL_FUNCTION_DECLARATIONS

        void AddToLookup(Element element, ElementAnimationData animationData)
        {
            int goInstanceId = element.gameObject.GetInstanceID();
            if (!elementFromToPairForAnimation.ContainsKey(goInstanceId))
                elementFromToPairForAnimation.Add(goInstanceId,new List<ElementAnimationData>());

            elementFromToPairForAnimation[goInstanceId].Add(animationData);

        }

        List<List<ElementAnimationData>> ConvertLookupToList()
        {
            List<List<ElementAnimationData>> animDataSortedByElement = new List<List<ElementAnimationData>>();

            foreach (var item in elementFromToPairForAnimation)
            {
                animDataSortedByElement.Add(item.Value);
            }

            

            return animDataSortedByElement;
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
                                AddToLookup(element, new ElementAnimationData(element, currentCell, bottomFromCurrent, executionData));
                                currentCell = bottomFromCurrent;
                                continue;
                            }



                            whileSafeCheck++;

                            if (whileSafeCheck >= 900)
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

        void ShiftCellsRightAndDown() {

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
                                AddToLookup(element, new ElementAnimationData(element, currentCell, bottomFromCurrent, executionData));
                                currentCell = bottomFromCurrent;
                                continue;
                            }

                            if (bottomRightFromCurrent != null)
                            {
                                gI++;
                                gJ++;
                                Element element = currentCell.GetElement();
                                bottomRightFromCurrent.SetElement(element);
                                AddToLookup(element, new ElementAnimationData(element, currentCell, bottomRightFromCurrent, executionData));
                                currentCell = bottomRightFromCurrent;
                                continue;
                            }


                            whileSafeCheck++;

                            if (whileSafeCheck >= 900)
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

        void ShiftCellsLeftAndDown() {

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
                                AddToLookup(element, new ElementAnimationData(element, currentCell, bottomFromCurrent, executionData));
                                currentCell = bottomFromCurrent;
                                continue;
                            }

                            if (bottomLeftFromCurrent != null)
                            {

                                gI++;
                                gJ--;
                                Element element = currentCell.GetElement();
                                bottomLeftFromCurrent.SetElement(element);
                                AddToLookup(element, new ElementAnimationData(element, currentCell, bottomLeftFromCurrent, executionData));
                                currentCell = bottomLeftFromCurrent;
                                continue;

                            }

                            whileSafeCheck++;

                            if (whileSafeCheck >= 900)
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

                List<Vector3> positions = new List<Vector3>();
                List<GridCell> targetCells = new List<GridCell>();

                for (int j = 0; j < grid.GridHeight; j++)
                {
                    GridCell currentCell = grid[j, i];

                    if (currentCell == null)
                        continue;

                    if (!currentCell.IsEmpty)
                        break;

                    position.y += GridDesignTemp.gridSpacing;
                    positions.Insert(0, position);
                    targetCells.Add(currentCell);
                }

                for (int k = 0; k < positions.Count; k++)
                {
                    Element newElement = GenerateElementAt(targetCells[k]);
                    newElement.transform.position = positions[k];
                  //  elementFromToPairForAnimation.Add(new ElementAnimationData(newElement, targetCells[k], targetCells[k], executionData));
                }
            }
        }

        void AnimateMovement() {

            StartCoroutine(AnimateMovementRoutine());
        }


        IEnumerator AnimateMovementRoutine()
        {
            
            //Same animationData.ToCell is making problem since both execute together,
            // SCan for same To tarets and remove that ins chain entirely to a new chain
            // and make sure the one being removed is with lesser HIndex compared to other one or overlapping will still persist

            WaitForSeconds interAnimationChainDispatchDelay = new WaitForSeconds(0.02f);
            List<List<ElementAnimationData>> animDataSortedByElement = ConvertLookupToList();

            LogChain(animDataSortedByElement,"Before");

            for (int i = 0; i < animDataSortedByElement.Count;i++)
            {
                StartCoroutine(AnimateElementChain(animDataSortedByElement[i]));
                yield return interAnimationChainDispatchDelay;
            }

           yield return null;
        }
        

        }

        IEnumerator AnimateElementChain(List<ElementAnimationData> elementAnimationDatas) {


            for (int i = 0; i < elementAnimationDatas.Count; i++)
            {
                yield return elementAnimationDatas[i].Animate();
            }

            yield return null;
        }



   
    private void LogChain(List<List<ElementAnimationData>> animDataSortedByElement,string msg)
    {
        string vals = string.Empty;
        string seperator = "|";

        for (int j = 0; j < animDataSortedByElement.Count; j++)
        {
            for (int i = 0; i < animDataSortedByElement[j].Count; i++)
            {
                vals += "insId"+animDataSortedByElement[j][i].elementName+":"+animDataSortedByElement[j][i].FromCell.gameObject.name + "->" + animDataSortedByElement[j][i].ToCell.gameObject.name + seperator;
            }
            v += vals + "\n";
        }

        Debug.Log(msg + v);
        v = String.Empty;
    }


    void SortListByMaxHIndex(List<List<ElementAnimationData>> animDatasSortedByElement)
    {

        for (int i = 0; i < animDatasSortedByElement.Count; i++)
        {
            for (int j = i; j < animDatasSortedByElement.Count; j++)
            {
                int v1 = GetMaxOf(animDatasSortedByElement[i]);
                int v2 = GetMaxOf(animDatasSortedByElement[j]);

                if (v1 < v2) 
                {
                    var temp = animDatasSortedByElement[i];
                    animDatasSortedByElement[i] = animDatasSortedByElement[j];
                    animDatasSortedByElement[j] = temp;
                }
            }
        }



        int GetMaxOf(List<ElementAnimationData> dt)
        {
            int max = dt[0].ToCell.HIndex;
            for (int i = 1; i < dt.Count; i++)
            {
                int tm = dt[i].ToCell.HIndex;

                if (tm > max)
                    max = tm;
            }

            return max;
        }
        
       


    }
      
    #endregion


    string v = string.Empty;

}

public class ElementAnimationData
{

    public Element Element;
    public GridCell FromCell;
    public GridCell ToCell;
    public string elementName;
    public bool isAnimating= false; 

    public List<ElementAnimationData> waitForAnimation = new List<ElementAnimationData>();

    public ElementAnimationData(Element element, GridCell fromCell, GridCell toCell, MatchExecutionData currentExecutionData)
    {
        Element = element;
        FromCell = fromCell;
        ToCell = toCell;
        elementName = element.GetInstanceID().ToString();

        if (toCell.executionData == null)
            toCell.SetExecutionData(currentExecutionData);

        
    }

    public IEnumerator Animate() 
    {
        for (int i = 0; i < waitForAnimation.Count; i++)
        {
            if (waitForAnimation[i].isAnimating)
                i=0;
        }

        yield return Element.AnimateToCellRoutine(ToCell);

    }

    public bool Equals(ElementAnimationData obj)
    {
        return Element.GetInstanceID() == obj.Element.GetInstanceID() &&
               FromCell.HIndex == obj.FromCell.HIndex
               && ToCell.HIndex == obj.ToCell.HIndex;
    }
}