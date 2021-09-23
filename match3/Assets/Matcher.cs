using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

public class MatchExecutionData
{
    private const int DefaultSwipeId=-99;
    public GridCell firstCell;
    public GridCell secondCell;
    public List<List<Element>> matchedElements;
    public List<GridCell> patternCells;
    public int swipeId;
    //public bool isAnimating;
    public float animationPeriod;
    
    public bool HasMatches
    {
        get
        {
            return matchedElements.Count > 0;
        }
    }

    public MatchExecutionData( List<List<Element>>matchedElementList,List<GridCell> patternCellList,int swipeNumber,GridCell fCell,GridCell sCell)
    {
        matchedElements = matchedElementList;
        patternCells = patternCellList;
        swipeId = swipeNumber;
        firstCell = fCell;
        secondCell = sCell;
        
        if(fCell!=null)
            animationPeriod = firstCell.ReadElement().elementData.swipeAnimationTime;
    }

    public bool Equals(MatchExecutionData obj)
    {
        if (obj==null)
            return false;
        
        if (obj.swipeId == DefaultSwipeId || swipeId==DefaultSwipeId)
        {
            return true;
        }

        return obj.swipeId == swipeId;

    }

    public override string ToString()
    {
        return $"Execution data id:{swipeId}";
    }

    
    public static MatchExecutionData GetDefaultExecutionData()
    {
        return new MatchExecutionData(new List<List<Element>>(), new List<GridCell>(), DefaultSwipeId,null,null);
    }


}

public class Matcher : Singleton<Matcher>
{
    
    [SerializeField] private MatchPattern[] patterns;

    public int swipeCount = 0;
    

    [ContextMenu("Start Checking")]
    public void StartChecking(MatchExecutionData executionData)
    {
        
        Debug.Log(executionData == null);
        FindMatches(executionData);
        
        if (executionData.HasMatches)
        {
            swipeCount++;
            WaitForGridAnimation(()=>
            {
                DestoryMatchesTillNoMatchPossible(executionData);
            },executionData);
        }
        else
        {
            WaitForGridAnimation(() =>
            {
                ReswapCells(executionData);
            },executionData);
        }
    }


    private void DestoryMatchesTillNoMatchPossible(MatchExecutionData executionData)
    {
        StartCoroutine(IterativeCheckRoutine(executionData));
    }

    private void ReswapCells(MatchExecutionData executionData)
    {
        InputManager.instance.SwapCells(executionData);
    }

    private IEnumerator IterativeCheckRoutine(MatchExecutionData executionData)
    {
        Grid grid = Grid.instance;
        
        while (executionData.HasMatches)
        {
            Debug.Log("ForceWait ");
            yield return new WaitForSeconds(2f);
            DestroyMatchedItems(executionData);
            grid.CollapseColoumns(executionData);
            yield return WaitForGridAnimationRoutine(executionData);
            FindMatches(executionData);
        }

        swipeCount--;
        while (swipeCount!=0)
        {
            yield return null;
        }
        
        grid.UnlockCells(executionData);
         
    }


    private void WaitForGridAnimation(Action action,MatchExecutionData executionData)
    {
        StartCoroutine(WaitForGridAnimationRoutine(executionData,action));
    }

    private IEnumerator WaitForGridAnimationRoutine(MatchExecutionData executionData,Action action=null)
    {
        Grid grid = Grid.instance;
        Debug.Log(executionData.animationPeriod);
        while (/*grid.IsAnimating ||*/ executionData.animationPeriod > 0)
        {
            executionData.animationPeriod -= 1;
            yield return new WaitForSeconds(1);
            
        }
        action?.Invoke();
    }


    private void DestroyMatchedItems(MatchExecutionData executionData)
    {
        List<List<Element>> matchedElements = executionData.matchedElements;
        
        for (int i = 0; i < matchedElements.Count;)
        {
            List<Element> sameElementsList = matchedElements[i];
            for (int j = 0; j < sameElementsList.Count; j++)
            {
                Destroy(sameElementsList[j].gameObject);
            }
            matchedElements.RemoveAt(i);
        }

       // matchExecutionDatas.Remove(executionData);
    }

    private void FindMatches(MatchExecutionData executionData)
    {
        
        Grid grid = Grid.instance;
     
        for (int i = 0; i < grid.GridHeight; i++)
        {
            for (int j = 0; j < grid.GridWidth; j++)
            {
                GridCell startingCell = grid[i, j];
                if (startingCell != null)
                {
                    for (int p = 0; p < patterns.Length; p++)
                    {
                        //Matching started for a single pattern 'p'
                        MatchPattern matchPattern = patterns[p];
                        
                        //We extract the cells according to pattern specified offsets
                        ExtractPatternCells(startingCell, matchPattern, i, j,executionData);
                        
                        if (!IsExtractionValid(executionData))
                        {
                            //Incase the geometry of grid doesnt allow us to continue,
                            // we skip the pattern for this 'Starting Cell' and check for next pattern 'p' at the 'Starting Cell'.
                            continue;
                        }
                        
                        //At this point we have cells with current pattern 'p', We can now safely check if 
                        //all the listed elements are same or not.
                        if (DoesSelectedCellsHaveSameElements(executionData)) 
                        {
                            //Debug.Log($"<Matcher> Cell count {_patternCells.Count}");
                            ExtractElementsToDestroyList(executionData);
                          //  grid.LockDirtyColoumns(executionData);
                           // Debug.Log("--------------------------------------------------------------");
                            //Debug.Log(".");
                            //Debug.Log(".");
                        }
                        //Pattern 'p' checked successfully here
                    }
                }
            }
        }

        //Debug.Log($"Total patterns detected {_matchedElements.Count}");

    }

    private void ExtractElementsToDestroyList(MatchExecutionData matchExecutionData)
    {
        Grid grid = Grid.instance;
        List<GridCell> patternCells = matchExecutionData.patternCells;
        List<Element> sameElementList = new List<Element>();
        for (int k = 0; k < patternCells.Count; k++)
        {
         //   Debug.Log($"<Matcher> Adding element fomr{_patternCells[k].gameObject.name} to matched list");
            Element element = patternCells[k].GetElement();
           //patternCells[k].isMarkedForDestory = true;
            sameElementList.Add(element);
        }
        grid.LockDirtyColoumns(matchExecutionData);
        matchExecutionData.matchedElements.Add(sameElementList);
        patternCells.Clear();
    }

    private bool IsExtractionValid(MatchExecutionData executionData)
    {
        List<GridCell> patternCells = executionData.patternCells;
        return (patternCells.Count > 0) ;
    }

    private bool DoesSelectedCellsHaveSameElements(MatchExecutionData matchExecutionData)
    {
        
        List<GridCell> patternCells = matchExecutionData.patternCells;
        
        Element startingElement = patternCells[0].ReadElement();
        for (int k = 1; k < patternCells.Count; k++)
        {
            Element patternCellElement = patternCells[k].ReadElement();
            if (!patternCellElement.IsSame(startingElement))
            {
          //      Debug.Log($"<Matcher> Terminating check due to different elements present at {_patternCells[k].gameObject.name} or is empty");
                patternCells.Clear();
                return false;
            }
        }

        return true;
    }

    private void ExtractPatternCells(GridCell startingCell,MatchPattern matchPattern, int i, int j,MatchExecutionData matchExecutionData)
    {
        List<GridCell> patternCells = matchExecutionData.patternCells;
        Grid grid = Grid.instance;
        
      //  Debug.Log($"<Matcher> Checking {matchPattern.patternName} at cell {startingCell.gameObject.name}");

        for (int k = 0; k < matchPattern.Length; k++) //Generate a list of cell from patterm
        {
            IndexPair offsetIndexPair = matchPattern[k];
            int iPaired = i + offsetIndexPair.i_Offset;
            int jPaired = j + offsetIndexPair.j_Offset;

            if ((iPaired >= grid.GridHeight || jPaired >= grid.GridWidth) ||( 
                grid[iPaired, jPaired] == null || grid[iPaired,jPaired].IsEmpty || grid[iPaired,jPaired].executionData !=null && 
                !grid[iPaired,jPaired].executionData.Equals(matchExecutionData)))
            {
                // Either grid geometry doesn't allow further check or previous pattern locked and extracted the cell thus current pattern will fail,
                // so we terminate execution instantly and clear the extractList;
                patternCells.Clear();
               
                //editor logging
                 //   Debug.Log($"<Matcher> Terminating check due i{iPaired} j{jPaired} >= {grid.GridHeight} {grid.GridWidth}");
                  //  Debug.Log($"<Matcher> OR");
                   // Debug.Log($"<Matcher> Terminating check due to no cell present at or is Empty {i}{j}");
                //end logging
                
                return;
            }
       
            GridCell cellOfPattern = grid[iPaired, jPaired];
            patternCells.Add(cellOfPattern);

        }

    }

    private static void LogCellsSelectedInPattern(List<GridCell> patternCells)
    {
        //DEB
        for (int k = 0; k < patternCells.Count; k++)
        {
            Debug.Log($"<Matcher> Cell selected {patternCells[k]}");
        }
        ////
    }
}
