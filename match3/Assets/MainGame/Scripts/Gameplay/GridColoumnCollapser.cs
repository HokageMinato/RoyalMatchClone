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
            elementSpawnPositions[i] = Instantiate(transformActivePrefab, parentTransform);
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


                Element newElement = GenerateElementAt(gridCell);
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
        if (executionData == null)
            throw new Exception("Null recievedd");

        HashSet<GridCell> occupiedCells = new HashSet<GridCell>();

        #region FUNCTION_EXECUTION_ORDER
        Grid grid = Grid.instance;
        Dictionary<int, List<ElementAnimationData>> elementFromToPairForAnimation = new Dictionary<int, List<ElementAnimationData>>();
        ShiftCells();

        AnimateMovement();
        #endregion


        #region LOCAL_FUNCTION_DECLARATIONS

        void AddToLookup(int element, ElementAnimationData animationData)
        {
            int goInstanceId = element;//.gameObject.GetInstanceID();
            if (!elementFromToPairForAnimation.ContainsKey(goInstanceId))
                elementFromToPairForAnimation.Add(goInstanceId, new List<ElementAnimationData>());

            elementFromToPairForAnimation[goInstanceId].Add(animationData);

        }

        List<ElementAnimationChain> ConvertLookupToList()
        {

            List<ElementAnimationChain> animDataSortedByElement = new List<ElementAnimationChain>();

            foreach (KeyValuePair<int, List<ElementAnimationData>> item in elementFromToPairForAnimation)
            {
                animDataSortedByElement.Add(new ElementAnimationChain(item.Value));
            }

            return animDataSortedByElement;
        }

        void ShiftCells()
        {
            int o = 0;
            for (int bI = grid.GridHeight - 1; bI >= 0; bI--)
            {
                for (int bJ = grid.GridWidth - 1; bJ >= 0; bJ--)
                {
                    int c = 0;

                    o++;
                    GridCell currentCell = grid[bI, bJ];
                    if (currentCell == null || currentCell.IsBlocked || currentCell.IsEmpty)
                        continue;

                    GridCell nextCell = GetNextCell(currentCell);

                    while (nextCell != null)
                    {
                        Element element = currentCell.GetElement();
                        nextCell.SetElement(element);


                        AddToLookup(o, new ElementAnimationData(element, currentCell, nextCell, executionData));

                        currentCell = nextCell;
                        nextCell = GetNextCell(currentCell);


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

            GridCell GetNextCell(GridCell currentCell)
            {
                GridCell nextCell=null;
                if (currentCell.bottomCell && currentCell.bottomCell.IsEmpty)
                {
                    nextCell = currentCell.bottomCell;
                    return nextCell;
                }

                if (currentCell.bottomRightCell && currentCell.bottomRightCell.IsEmpty && currentCell.rightCell && (currentCell.rightCell.IsEmpty || currentCell.rightCell.IsBlocked))
                {
                    nextCell = currentCell.bottomRightCell;
                    return nextCell;
                }

                if (currentCell.bottomLeftCell && currentCell.bottomLeftCell.IsEmpty && currentCell.leftCell && (currentCell.leftCell.IsEmpty || currentCell.leftCell.IsBlocked))
                {
                    nextCell = currentCell.bottomLeftCell;
                    return nextCell;
                }

                return nextCell;
                
            }
        }



    

        void AnimateMovement() {
            StartCoroutine(AnimateMovementRoutine());
        }

        IEnumerator AnimateMovementRoutine()
        {
            WaitForSeconds interAnimationChainDispatchDelay = new WaitForSeconds(.01f);
            List<ElementAnimationChain> animationChains = ConvertLookupToList();

           // LogChain(animationChains, "New");

            for (int i = 0; i < animationChains.Count; i++)
            {
                    StartCoroutine(AnimateElementChain(animationChains[i]));
                    yield return interAnimationChainDispatchDelay;
                

            }

            yield return null;
        }



        IEnumerator AnimateElementChain(ElementAnimationChain animationChain)
        {
            List<ElementAnimationData> elementAnimationDatas = animationChain.animationChain;

            for (int i = 0; i < elementAnimationDatas.Count; i++)
            {
                yield return elementAnimationDatas[i].Animate();
            }

            yield return null;
        }
    }
    #endregion


    private void LogChain(List<ElementAnimationChain> animDataSortedByElement, string msg)
        {
            Debug.Log(msg);
            string seperator = "|";

            for (int j = 0; j < animDataSortedByElement.Count; j++)
            {
                string vals = string.Empty;
                for (int i = 0; i < animDataSortedByElement[j].animationChain.Count; i++)
                {
                    vals += "insId" + animDataSortedByElement[j].animationChain[i].elementName + ":" + animDataSortedByElement[j].animationChain[i].FromCell.gameObject.name + "->" + animDataSortedByElement[j].animationChain[i].ToCell.gameObject.name + seperator;
                }
                Debug.Log(vals);
            }

        }
    
}




public class ElementAnimationChain
{

    public List<ElementAnimationData> animationChain;
    
    public ElementAnimationChain(List<ElementAnimationData> chain) 
    {
        animationChain = chain;    
    }

    public bool HasSideWaysAnimation 
    {
        get {

            foreach (var item in animationChain)
            {
                if (item.FromCell.HIndex != item.FromCell.WIndex)
                    return true;
            }
            return false;
        }
    }

    public GridCell[] GetCells() 
    {
        HashSet<GridCell> cells = new HashSet<GridCell>();

        for (int i = 0; i < animationChain.Count; i++)
        {
            cells.Add(animationChain[i].FromCell);
            cells.Add(animationChain[i].ToCell);
        }

        return cells.ToArray();
    }

}




public class ElementAnimationData
{

    public Element Element;
    public GridCell FromCell;
    public GridCell ToCell;
    public string elementName;
    public bool isAnimating= false; 

   

    public ElementAnimationData(Element element, GridCell fromCell, GridCell toCell, MatchExecutionData currentExecutionData)
    {
        Element = element;
        FromCell = fromCell;
        ToCell = toCell;
        elementName = element.GetInstanceID().ToString();

        if (currentExecutionData == null)
            Debug.Log("Null data");

        toCell.SetExecutionData(currentExecutionData);
        fromCell.SetExecutionData(currentExecutionData);
        
    }

    public IEnumerator Animate() 
    {
         yield return Element.AnimateToCellRoutine(ToCell);
    }

    public bool Equals(ElementAnimationData obj)
    {
        return Element.GetInstanceID() == obj.Element.GetInstanceID() &&
               FromCell.HIndex == obj.FromCell.HIndex
               && ToCell.HIndex == obj.ToCell.HIndex;
    }
}