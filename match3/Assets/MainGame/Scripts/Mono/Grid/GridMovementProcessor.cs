using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridMovementProcessor : MonoBehaviour
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

            if (cellType == GridConstants.NO_CELL || ObstacleFactory.instance.IsBlockerType(cellType))
                continue;

            Transform parentTransform = grid.GetLayerTransformParent(RenderLayer.ElementLayer);
            elementSpawnPositions[i] = Instantiate(transformActivePrefab, parentTransform);
            elementSpawnPositions[i].transform.position = grid[0, i].transform.position + Vector3.up * GridDesignTemp.GridSpacing;
        }
    }
    private void GenerateInitialElements() {
        Grid grid = Grid.instance;

        for (int i = 0; i < grid.GridHeight; i++)
        {
            for (int j = 0; j < grid.GridWidth; j++)
            {

                GridCell gridCell = grid[i, j];
                if (!gridCell || IsCellBlocked(gridCell))
                    continue;

                GenerateElementAt(gridCell);
            }
        }

    }

    public Dictionary<int,List<ElementAnimationData>> GenerateCollapseMovementData(MatchExecutionData executionData)
    {
        int uAnimId = 0;

        #region FUNCTION_EXECUTION_ORDER
        Grid grid = Grid.instance;
        Dictionary<int, List<ElementAnimationData>> elementFromToPairForAnimation = new Dictionary<int, List<ElementAnimationData>>();
        ShiftCells();
        ShiftNewCells();
        return elementFromToPairForAnimation;
        #endregion
        

        #region LOCAL_FUNCTION_DECLARATIONS

        void AddToLookup(int element, ElementAnimationData animationData)
        {
            int goInstanceId = element;
            if (!elementFromToPairForAnimation.ContainsKey(goInstanceId))
                elementFromToPairForAnimation.Add(goInstanceId, new List<ElementAnimationData>());

            elementFromToPairForAnimation[goInstanceId].Add(animationData);
        }

        void ShiftCells()
        {
           
            for (int bI = grid.GridHeight - 1; bI >= 0; bI--)
            {
                for (int bJ = grid.GridWidth - 1; bJ >= 0; bJ--)
                {
                    uAnimId++;
                    
                    GridCell currentCell = grid[bI, bJ];
                    if (currentCell == null || IsCellBlocked(currentCell) || currentCell.IsEmpty)
                        continue;

                    GridCell nextCell = GetNextCellForShifting(currentCell);
                    
                    int c = 0;
                    while (nextCell != null)
                    {
                        Element element = currentCell.GetElement();
                        nextCell.SetElement(element);

                        AddToLookup(uAnimId, new ElementAnimationData(element, currentCell, nextCell, executionData,bJ));
                        currentCell = nextCell;
                        nextCell = GetNextCellForShifting(currentCell);

                        
                        #region INF_SAFE_CHECK
                        c++;
                        if (c > 900)
                        {
                            Debug.Log("INFI LOOP");
                            break;
                        }
                        #endregion
                    }

                }
                
            }


        }

        void ShiftNewCells()
        {
            float heightStack = 0f;
            int oc = 0;
            for (int bJ = 0; bJ < grid.GridWidth;)
            {
                
                

                GridCell currentCell = grid[0, bJ];
                if (currentCell == null || !currentCell.IsEmpty || IsCellBlocked(currentCell))
                {
                    bJ++;
                    continue;
                }


                GenerateElementAt(currentCell);
                Element newElement = currentCell.ReadElement();
                heightStack += GridDesignTemp.GridSpacing;
                newElement.transform.position = currentCell.transform.position + (Vector3.up * heightStack);
                uAnimId++;

                GridCell nextCell = GetNextCellForShifting(currentCell);
                int c = 0;
                while (nextCell != null)
                {
                    Element element = currentCell.GetElement();
                    nextCell.SetElement(element);

                    AddToLookup(uAnimId, new ElementAnimationData(element, currentCell, nextCell, executionData,bJ));
                    currentCell = nextCell;
                    nextCell = GetNextCellForShifting(currentCell);
                    #region INF_SAFE_CHECK
                    c++;
                    if (c > 900)
                    {
                        Debug.Log("INFI LOOP 180");
                        break;
                    }
                    #endregion
                }


                AddToLookup(uAnimId, new ElementAnimationData(newElement, currentCell, currentCell, executionData,bJ));

                if (!IsColoumnEmpty(bJ))
                {
                    bJ++;
                    heightStack = 0f;
                    
                    //next coloumn
                }


                #region INF_SAFE_CHECK

                oc++;
                if (oc > 900)
                {
                    Debug.Log("INFI LOOP at 192");
                    break;
                }
                #endregion

            }

        }




        GridCell GetNextCellForShifting(GridCell currentCell)
        {
            GridCell nextCell = null;
            if (currentCell.bottomCell && currentCell.bottomCell.IsEmpty)
            {
                nextCell = currentCell.bottomCell;
                return nextCell;
            }

            if (currentCell.bottomRightCell && currentCell.bottomRightCell.IsEmpty && !PendingElementsCheckFromBottomRecursive(currentCell.bottomRightCell))
            {
                nextCell = currentCell.bottomRightCell;
                return nextCell;
            }

            if (currentCell.bottomLeftCell && currentCell.bottomLeftCell.IsEmpty && !PendingElementsCheckFromBottomRecursive(currentCell.bottomLeftCell))
            {
                nextCell = currentCell.bottomLeftCell;
                return nextCell;
            }

            return nextCell;

        }

      

        bool PendingElementsCheckFromBottomRecursive(GridCell initialCell)
        {
            GridCell presentCell = initialCell;

            int c = 0;
            while (presentCell.topCell != null)
            {
                presentCell = presentCell.topCell;

                if (!presentCell.IsEmpty)
                    return true;


                #region INF_SAFE_CHECK
                c++;
                if (c > 900)
                {
                    Debug.Log("INFI LOOP");
                    break;
                }
                #endregion
            }

            return false;
        }
        #endregion
    }

    bool IsColoumnEmpty(int colIdx) 
    { 
        return Grid.instance.IsColoumnEmpty(colIdx);
    }
   
    private bool IsCellBlocked(GridCell gridCell)
    {
        return GameplayObstacleHandler.instance.IsCellBlocked(gridCell);
    }

   

    private void GenerateElementAt(GridCell gridCell)
    {
        ElementFactory elementFactory = ElementFactory.instance;
        Element newElement = elementFactory.GenerateRandomGameplayElement();
        gridCell.SetElement(newElement);
        newElement.transform.position = gridCell.transform.position;
    }



}


public class ElementAnimationData
{

    public Element Element;
    public GridCell FromCell;
    public GridCell ToCell;
    public string elementName;
    public int ColIdx;
   

    public ElementAnimationData(Element element, GridCell fromCell, GridCell toCell, MatchExecutionData currentExecutionData,int colIdx)
    {
        Element = element;
        FromCell = fromCell;
        ToCell = toCell;
        elementName = element.GetInstanceID().ToString();
        ColIdx = colIdx;

        if (currentExecutionData == null)
            Debug.Log("Null data");

        toCell.SetExecutionData(currentExecutionData);
        fromCell.SetExecutionData(currentExecutionData);

    }

    public IEnumerator Animate() 
    {
         yield return Element.AnimateToCellRoutine(ToCell.transform);
    }

   
}