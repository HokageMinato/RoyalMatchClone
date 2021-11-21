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
      
      return elementConfig.Equals(other.elementConfig);
   }

   public void AnimateToCell(GridCell newHolder)
   {
      StartCoroutine(AnimateToCellRoutine(newHolder));
   }
    
   // public void AnimateToCell(GridCell newHolder, List<GridCell> travelPath=null)
   //{
   //   StartCoroutine(AnimateToCellRoutine(newHolder,travelPath));
   //}



    //private IEnumerator AnimateToCellRoutine(GridCell newHolder,List<GridCell> travelPath=null)
    //{

    //    MatchExecutionData executionData = newHolder.executionData;
    //    executionData.movingElements.Add(this);

    //    if (travelPath != null)
    //        for (int p = 0; p < travelPath.Count; p++)
    //        {
    //            yield return ShiftToCell(travelPath[p]);
    //        }
    //    else
    //        yield return ShiftToCell(newHolder);

        
    //    executionData.movingElements.Remove(this);
    //}
    
   
    public IEnumerator AnimateToCellRoutine(GridCell newHolder)
    {
        MatchExecutionData executionData = newHolder.executionData;
        executionData.movingElements.Add(this);

        float rate = 1 / ElementConfig.SWIPE_ANIM_TIME;
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
