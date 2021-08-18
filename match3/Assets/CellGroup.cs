using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGroup : MonoBehaviour
{

    public GridCell[] groupedCells;


    public void OnMoveDone(int[] w, int[]h)
    {
        bool isNear = false;
        
        for (int i = 0; i < groupedCells.Length; i++)
        {
            for (int j = 0; j < w.Length; j++)
            {
               // Mathf.Abs(groupedCells[i].HIndex-h[j]) < 1
            }

            for (int j = 0; j < h.Length; j++)
            {
                
            }
        }
    }

}
