using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Element : MonoBehaviour
{
   public ElementType elementType;
   public GridCell holder;

   [SerializeField] private GridCell previousHolder=null;

   
   public bool IsSame(Element other)
   {
      return other.elementType == elementType;
   }

   public void SetHolder(GridCell newHolder)
   {
      previousHolder = holder;
      holder = newHolder;
      holder.element = this;
      transform.SetParent(holder.transform);
      StartCoroutine(SwipeRoutine());
   }

   private IEnumerator SwipeRoutine()
   {
      float rate = 1f / .45f;
      float i = 0;
      
      while (i <= 1)
      {
         transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, i);
         i += rate * Time.deltaTime;
         yield return null;
      }

   }

   public enum ElementType
   {
      Type1,
      Type2,
      Type3,
      Type4
   }
}
