using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridColoumn : MonoBehaviour
{
   public List<GridCell> gridCells;
   public ElementGenerator elementGenerator;
   
   private List<Element> _generatedElementList = new List<Element>();
   private int cellIndex = 0;
   
   public void CollapseColoumn()
   {
         ShiftRemainingCellsToEmptySpaces();
         GenerateNewElementBuffer();
         SetNewlyGeneratedElementsToEmptyCells();
         StartCoroutine(WaitForGridAnimation(UnLockColoumn));
      
   }

   private void SetNewlyGeneratedElementsToEmptyCells()
   {
      for (int i = 0; i < _generatedElementList.Count; i++)
      {
         GridCell cell = gridCells[cellIndex];
         Element element = _generatedElementList[i];
         cell.SetElement(element);
         cellIndex--;
      }

   }

   private void GenerateNewElementBuffer()
   {
      _generatedElementList.Clear();
      for (int i = 0; i < gridCells.Count; i++)
      {
         GridCell cell = gridCells[i];

         if (cell.IsEmpty)
         {
            Element element = elementGenerator.GetRandomElement(cell);
            Transform elementTransform = element.transform;

            Vector3 initialPosition = elementTransform.InverseTransformPoint(elementGenerator.transform.localPosition);
            initialPosition.y += i;
            elementTransform.localPosition = initialPosition;
            _generatedElementList.Add(element);
         }
      }
   }

   private void ShiftRemainingCellsToEmptySpaces()
   {
      cellIndex = gridCells.Count - 1;
      for (int i = gridCells.Count - 1; i >= 0; i--)
      {
         Element element = gridCells[i].GetElement();
         if (element != null)
         {
            gridCells[cellIndex].SetElement(element);
            cellIndex--;
         }
      }
   }


   public void AddCell(GridCell newCell)
   {
      gridCells.Add(newCell);
   } 
   
   public void SetGenerator(ElementGenerator elementGenerat)
   {
      elementGenerator = elementGenerat;
      elementGenerator.transform.SetParent(transform);
   }

   public void LockColoumn()
   { 
      for (int i = 0; i < gridCells.Count; i++)
      {
            gridCells[i].ToggleInputInteractibility(false);
      }
   }
    
   public void UnLockColoumn()
   {
      for (int i = 0; i < gridCells.Count; i++)
      {
         gridCells[i].ToggleInputInteractibility(true);
         gridCells[i].isMarkedForDestory = false;
      }
   }

   public bool IsColoumnDirty()
   {
      for (int i = 0; i < gridCells.Count; i++)
      {
         if (gridCells[i].isMarkedForDestory)
            return true;
      }

      return false;
   }
   
   
   
   IEnumerator WaitForGridAnimation(Action onAnimationDone)
   {
      while (Grid.instance.IsAnimating)
         yield return null;

      onAnimationDone();
   }

   public void SetExecutionData(MatchExecutionData executionData)
   {
      for (int i = 0; i < gridCells.Count; i++)
      {
         gridCells[i].SetExecutionData(executionData);

      }
   }
}
