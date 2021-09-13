using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

public class Matcher : Singleton<Matcher>
{
    
    [SerializeField] private MatchPattern[] patterns;
    private readonly List<List<Element>> _matchedElements = new List<List<Element>>();
    private readonly List<GridCell> _patternCells = new List<GridCell>();
    
    public bool HasMatches
    {
        get
        {
            return _matchedElements.Count > 0;
        }
    }

    [ContextMenu("Start Checking")]
    public void StartChecking()
    {
        FindMatches();
        if(HasMatches)
            StartCoroutine(IterativeCheckRoutine());
    }

    private IEnumerator IterativeCheckRoutine()
    {
        Grid grid = Grid.instance;
        yield return WaitForGridAnimation();
        
      //I  Debug.Log($"<Matcher> Matched elements count{_matchedElements.Count}");
        while (_matchedElements.Count > 0)
        {
            DestroyMatchedItems();
            grid.LockColoumns();
            yield return new WaitForSeconds(.2f);
            grid.CollapseColoumns();
            yield return WaitForGridAnimation();
            grid.UnlockColoumns();
            FindMatches();
        }

    }

    

    private IEnumerator WaitForGridAnimation()
    {
        Grid grid = Grid.instance;
        while (grid.IsAnimating)
        {
            yield return null;
        }
    }


    private void DestroyMatchedItems()
    {
        for (int i = 0; i < _matchedElements.Count;)
        {
            List<Element> sameElementsList = _matchedElements[i];
            for (int j = 0; j < sameElementsList.Count; j++)
            {
                Destroy(sameElementsList[j].gameObject);
            }
            _matchedElements.RemoveAt(i);
        }
    }

    private void FindMatches()
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
                        ExtractPatternCells(startingCell, matchPattern, i, j);
                        if (!IsExtractionValid())
                        {
                            //Incase the geometry of grid doesnt allow us to continue,
                            // we skip the pattern for this 'Starting Cell' and check for next pattern 'p' at the 'Starting Cell'.
                            continue;
                        }
                        
                        //At this point we have cells with current pattern 'p', We can now safely check if 
                        //all the listed elements are same or not.
                        if (DoesSelectedCellsHaveSameElements()) 
                        {
                            //Debug.Log($"<Matcher> Cell count {_patternCells.Count}");
                            ExtractElementsToDestroyList();
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

    private void ExtractElementsToDestroyList()
    {
        List<Element> sameElementList = new List<Element>();
        for (int k = 0; k < _patternCells.Count; k++)
        {
         //   Debug.Log($"<Matcher> Adding element fomr{_patternCells[k].gameObject.name} to matched list");
            Element element = _patternCells[k].GetElement();
            sameElementList.Add(element);
           //element.gameObject.SetActive(false); //TODO remove later
        }
        _matchedElements.Add(sameElementList);
        _patternCells.Clear();
    }

    private bool IsExtractionValid()
    {
        return (_patternCells.Count > 0) ;
    }

    private bool DoesSelectedCellsHaveSameElements()
    {
        Element startingElement = _patternCells[0].ReadElement();
        for (int k = 1; k < _patternCells.Count; k++)
        {
            Element patternCellElement = _patternCells[k].ReadElement();
            if (!patternCellElement.IsSame(startingElement))
            {
          //      Debug.Log($"<Matcher> Terminating check due to different elements present at {_patternCells[k].gameObject.name} or is empty");
                _patternCells.Clear();
                return false;
            }
        }

        return true;
    }

    private void ExtractPatternCells(GridCell startingCell,MatchPattern matchPattern, int i, int j)
    {
        
        Grid grid = Grid.instance;
        
      //  Debug.Log($"<Matcher> Checking {matchPattern.patternName} at cell {startingCell.gameObject.name}");

        for (int k = 0; k < matchPattern.Length; k++) //Generate a list of cell from patterm
        {
            IndexPair offsetIndexPair = matchPattern[k];
            int iPaired = i + offsetIndexPair.i_Offset;
            int jPaired = j + offsetIndexPair.j_Offset;

            if ((iPaired >= grid.GridHeight || jPaired >= grid.GridWidth) ||( 
                grid[iPaired, jPaired] == null || grid[iPaired,jPaired].IsEmpty))
            {
                // Either grid geometry doesn't allow further check or previous pattern locked and extracted the cell thus current pattern will fail,
                // so we terminate execution instantly and clear the extractList;
                _patternCells.Clear();
               
                //editor logging
                 //   Debug.Log($"<Matcher> Terminating check due i{iPaired} j{jPaired} >= {grid.GridHeight} {grid.GridWidth}");
                  //  Debug.Log($"<Matcher> OR");
                   // Debug.Log($"<Matcher> Terminating check due to no cell present at or is Empty {i}{j}");
                //end logging
                
                return;
            }
       
            GridCell cellOfPattern = grid[iPaired, jPaired];
            _patternCells.Add(cellOfPattern);
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
