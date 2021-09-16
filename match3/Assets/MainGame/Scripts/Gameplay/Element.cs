using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Element : MonoBehaviour
{
   public ElementType elementType;
   public float swipeAnimTime;
   public AnimationCurve curve;
   
   public bool IsSame(Element other)
   {
      if (other == null)
         return false;
      
      return other.elementType == elementType;
   }

   public void DestroySelf()
   {
      Destroy(gameObject);
   }

 
   public void SetHolder(GridCell newHolder)
   {
      transform.SetParent(newHolder.transform);
      StartCoroutine(ShiftRoutine());
   }

   private IEnumerator ShiftRoutine()
   {
      Grid.instance.IsAnimating = true;
      float rate = 1f/swipeAnimTime ;
      float i = 0;
      Vector3 sourcePosition = transform.localPosition;
      Vector3 pseudoZero = new Vector3(0.001f, 0.001f,0.01f);
      
      while (i <= 1f)
      {
         i += rate * Time.deltaTime;
         transform.localPosition = Vector3.Lerp(sourcePosition, pseudoZero, i);
         yield return null;
      }
   
      Grid.instance.IsAnimating = false;
   
   }
   [ContextMenu("Test")]
   public void Move()
   {
      StartCoroutine(ShiftRoutine());
   }
   //speed based
   // private IEnumerator ShiftRoutine()
   // {
   //    Grid.instance.IsAnimating = true;
   //    
   //    Vector3 pseudoZero = new Vector3(0.01f,0.01f,0.01f);
   //    
   //    Vector3 distance = (pseudoZero - transform.localPosition);
   //    while (distance.magnitude > 0.0001f)
   //    {
   //       distance = (pseudoZero - transform.localPosition);
   //       transform.localPosition += distance * Time.deltaTime * swipeAnimTime;
   //       yield return null;
   //    }
   //
   //    yield return null;
   //
   //    Grid.instance.IsAnimating = false;
   //
   // }

   
   public enum ElementType
   {
      Type1,
      Type2,
      Type3,
      Type4
   }
}
