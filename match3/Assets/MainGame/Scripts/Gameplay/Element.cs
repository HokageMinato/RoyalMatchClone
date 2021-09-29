using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Element : MonoBehaviour
{
   
   
   public const float SWIPE_ANIM_TIME = 0.2f;
  
   
   private MatchExecutionData _matchExecutionData;
   public ElementType elementType;
   
   public bool IsSame(Element other)
   {
      if (other == null)
         return false;
      
      return elementType==other.elementType;
   }

   public void SetHolder(GridCell newHolder)
   {
      _matchExecutionData = newHolder.executionData;
      transform.SetParent(newHolder.transform);
      StartCoroutine(ShiftRoutine());
   }

   private IEnumerator ShiftRoutine()
   {
       
      float rate = 1f/SWIPE_ANIM_TIME;
      float i = 0;
      Vector3 sourcePosition = transform.localPosition;
      Vector3 pseudoZero = new Vector3(0.001f, 0.001f,0.01f);
      
      while (i <= 1f)
      {
         i += rate * Time.deltaTime;
         transform.localPosition = Vector3.Lerp(sourcePosition, pseudoZero, i);
         yield return null;
      }
   
   
   }
   [ContextMenu("Test")]
   public void Move()
   {
      StartCoroutine(ShiftRoutine());
   }
   

   public enum ElementType
   {
      Type1,
      Type2,
      Type3,
      Type4
   }
  
}
