using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

public class Element : MonoBehaviour
{

   [SerializeField] ElementConfig elementConfig;
   IEnumerator animateRoutine;
    
    
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
        animateRoutine =  (AnimateToCellRoutine(newHolder));
        StartCoroutine(animateRoutine);
   }

    public void DestroyElement() 
    {
        DestroyImmediate(gameObject);
    }
   
    public IEnumerator AnimateToCellRoutine(GridCell newHolder)
    {
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

        animateRoutine = null;
        
    }


    


}
