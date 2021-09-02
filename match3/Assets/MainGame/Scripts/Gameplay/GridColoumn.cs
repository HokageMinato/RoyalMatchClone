using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridColoumn : MonoBehaviour
{
   public List<GridCell> gridCells;
   
   

   public void CollapseColoumn()
   {
      
      List<Element> elements = new List<Element>();
      
      for (int i = gridCells.Count-1; i >= 0; i--)
      {
         Element element = gridCells[i].GetElement();
         if (element != null)
         {
            elements.Add(element);
         }
      }

      int cellIndex = gridCells.Count - 1;
      for (int i = 0; i < elements.Count; i++)
      {
       //  Debug.Log(elements[i].gameObject.name +" AT "+ gridCells[cellIndex].gameObject.name );
         gridCells[cellIndex].SetElement(elements[i]);
         cellIndex--;
      }      
   }

   public bool IsAnimating()
   {
      for (int i = 0; i < gridCells.Count; i++)
      {
         if (gridCells[i].IsAnimating())
            return true;
      }

      return false;

   }

   public void AddCell(GridCell newCell)
   {
      gridCells.Add(newCell);
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
      }
   }


   
}
