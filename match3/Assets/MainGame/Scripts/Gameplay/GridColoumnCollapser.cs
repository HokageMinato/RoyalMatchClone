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
        element.initial = cell;
        return element;
    }

    public void CollapseColomuns(MatchExecutionData executionData) 
    {
        if (executionData == null)
            throw new Exception("Null recievedd");

        #region FUNCTION_EXECUTION_ORDER
        Grid grid = Grid.instance;

        Dictionary<int, List<ElementAnimationData>> elementFromToPairForAnimation = new Dictionary<int, List<ElementAnimationData>>();
        ShiftCells();
         
       
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
            for (int bI = grid.GridHeight - 1; bI >= 0; bI--)
            {
                for (int bJ = grid.GridWidth - 1; bJ >= 0; bJ--)
                {
                    int c = 0;


                    GridCell currentCell = grid[bI, bJ];
                    if (currentCell == null || currentCell.IsBlocked || currentCell.IsEmpty)
                        continue;

                    GridCell nextCell = GetNextCell(currentCell);

                    while (nextCell != null)
                    {
                        Element element = currentCell.GetElement();
                        nextCell.SetElement(element);

                        AddToLookup(element, new ElementAnimationData(element, currentCell, nextCell, executionData));
                        
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
                GridCell nextCell;
                if (currentCell.bottomCell && !currentCell.bottomCell.IsBlocked && currentCell.bottomCell.IsEmpty)
                    nextCell = currentCell.bottomCell;

                else if (currentCell.bottomLeftCell && !currentCell.bottomLeftCell.IsBlocked && currentCell.bottomLeftCell.IsEmpty)
                    nextCell = currentCell.bottomLeftCell;

                else if (currentCell.bottomRightCell && !currentCell.bottomRightCell.IsBlocked && currentCell.bottomRightCell.IsEmpty)
                    nextCell = currentCell.bottomRightCell;
                else
                    nextCell = null;
                return nextCell;
            }
        } 
        
        

        void AnimateMovement() {

            StartCoroutine(AnimateMovementRoutine());
        }


        IEnumerator AnimateMovementRoutine()
        {
            WaitForSeconds interAnimationChainDispatchDelay = new WaitForSeconds(0.02f);
            List<ElementAnimationChain> animDataSortedByElement = ConvertLookupToList();


            // SortChainsByLength(animDataSortedByElement);
            LogChain(animDataSortedByElement,"New");
            for (int i = 0; i < animDataSortedByElement.Count;i++)
            {
                StartCoroutine(AnimateElementChain(animDataSortedByElement[i]));
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

    private void LogChain(List<ElementAnimationChain> animDataSortedByElement,string msg)
   {
        Debug.Log(msg);
        string seperator = "|";

        for (int j = 0; j < animDataSortedByElement.Count; j++)
        {
            string vals = string.Empty;
            for (int i = 0; i < animDataSortedByElement[j].animationChain.Count; i++)
            {
                vals += "insId"+animDataSortedByElement[j].animationChain[i].elementName+":"+animDataSortedByElement[j].animationChain[i].FromCell.gameObject.name + "->" + animDataSortedByElement[j].animationChain[i].ToCell.gameObject.name + seperator;
            }
            Debug.Log(vals);
        }

   }


    void SortChainsByLength(List<ElementAnimationChain> animDatasSortedByElement)
    {

        for (int i = 0; i < animDatasSortedByElement.Count; i++)
        {
            for (int j = i; j < animDatasSortedByElement.Count; j++)
            {
                int v1 = GetMaxOf(animDatasSortedByElement[i].animationChain);
                int v2 = GetMaxOf(animDatasSortedByElement[j].animationChain);

                if (v1 > v2) 
                {
                    var temp = animDatasSortedByElement[i];
                    animDatasSortedByElement[i] = animDatasSortedByElement[j];
                    animDatasSortedByElement[j] = temp;
                }
            }
        }



        int GetMaxOf(List<ElementAnimationData> dt)
        {
            //int max = dt[0].ToCell.HIndex;
            //for (int i = 1; i < dt.Count; i++)
            //{
            //    int tm = dt[i].ToCell.HIndex;

            //    if (tm > max)
            //        max = tm;
            //}

            return dt.Count;
        }
        
       


    }

    void SortChainsByMaxH(List<ElementAnimationChain> animDatasSortedByElement)
    {

        for (int i = 0; i < animDatasSortedByElement.Count; i++)
        {
            for (int j = i; j < animDatasSortedByElement.Count; j++)
            {
                int v1 = GetMaxOf(animDatasSortedByElement[i].animationChain);
                int v2 = GetMaxOf(animDatasSortedByElement[j].animationChain);

                if (v1 > v2)
                {
                    var temp = animDatasSortedByElement[i];
                    animDatasSortedByElement[i] = animDatasSortedByElement[j];
                    animDatasSortedByElement[j] = temp;
                }
            }
        }



        int GetMaxOf(List<ElementAnimationData> dt)
        {
            return dt[dt.Count-1].ToCell.HIndex;
        }




    }

    #endregion



}

public class ElementAnimationChain
{

    public List<ElementAnimationData> animationChain;
    
    public ElementAnimationChain(List<ElementAnimationData> chain) 
    {
        animationChain = chain;    
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