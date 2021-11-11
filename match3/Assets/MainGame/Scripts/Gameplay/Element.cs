using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Element : MonoBehaviour
{

    [SerializeField] ElementConfig elementConfig;

    public RenderLayer RenderLayer
    {
        get {
            return ElementConfig.renderLayer;
        }
    }
    
    

    public bool Equals(Element other)
   {
      if (other == null)
         return false;
      
      return elementConfig.elementId.Equals(other.elementConfig.elementId);
   }

   public void SetHolder(GridCell newHolder)
   {
      StartCoroutine(ShiftRoutine(newHolder));
   }

    public void SetHolderWithPath(GridCell newHolder,List<GridCell> travelPath) {
   
    }

    private IEnumerator ShiftRoutine(GridCell newHolder)
    {

        MatchExecutionData executionData = newHolder.executionData;
        executionData.movingElements.Add(this);


        float rate = 1f / ElementConfig.SWIPE_ANIM_TIME;
        float i = 0;
        Vector3 sourcePosition = transform.position;
        Vector3 destinationPosition = newHolder.transform.position;

        while (i <= 1f)
        {
            i += rate * Time.deltaTime;
            transform.localPosition = Vector3.Lerp(sourcePosition, destinationPosition, i);
            yield return null;
        }

        executionData.movingElements.Remove(this);
    }
    
   
  

   
  
}
