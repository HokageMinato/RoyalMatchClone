using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Element : MonoBehaviour
{
   public ElementType elementType;
   public float swipeAnimTime;
   //private bool isAnimating;
   
   // public bool IsAnimating
   // {
   //    get
   //    {
   //       return isAnimating;
   //    }
   //
   // }

   public bool IsSame(Element other)
   {
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
    //  isAnimating = true;
      Grid.instance.IsAnimating = true;
      float rate = 1f/swipeAnimTime ;
      float i = 0;
      Vector3 sourcePosition = transform.localPosition;
      Vector3 pseudoZero = new Vector3(0.01f, 0.01f,0.01f);
      
      while (i <= 1f)
      {
         i += rate * Time.deltaTime;
         transform.localPosition = Vector3.Lerp(sourcePosition, pseudoZero, i);
         yield return null;
      }

        // isAnimating = false;
         Grid.instance.IsAnimating = false;

   }

   
   public enum ElementType
   {
      Type1,
      Type2,
      Type3,
      Type4
   }
}
