using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GridCell : MonoBehaviour,IPointerDownHandler,IPointerEnterHandler
{
    public GridCell elementSource;


    public void OnPointerDown(PointerEventData eventData)
    {
        SwipeInputReciever.Instance.selectedCell = this;
        SwipeInputReciever.Instance.secondCell = null;
        Debug.Log("Clicking");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(SwipeInputReciever.Instance.selectedCell != this)
            SwipeInputReciever.Instance.secondCell = this;
        
        //TODO Create a method in GridCell to check weather the second cell is neighbourCell or not, if so assign else discard.
    }
}
