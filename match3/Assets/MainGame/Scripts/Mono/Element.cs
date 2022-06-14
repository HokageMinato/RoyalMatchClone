using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

public class Element : MonoBehaviour
{
   [SerializeField] private ElementConfig elementConfig;
   private IEnumerator animateRoutine;

   public ElementConfig ElementConfig { get { return elementConfig; } }   
   
   public RenderLayer RenderLayer
   {
        get 
        {
            return ElementConfig.renderLayer;
        }
   }

   
   public bool Equals(Element other)
   {
      if (other == null)
         return false;
      
      return elementConfig.Equals(other.elementConfig);
   }

   

   public void AnimateToCell(Transform newHolder)
   {
        animateRoutine =  AnimateToCellRoutine(newHolder);
        StartCoroutine(animateRoutine);
   }

    public void DestroyElement() 
    {
        DestroyImmediate(gameObject);
    }
   
    public IEnumerator AnimateToCellRoutine(Transform newHolder)
    {
        float rate = 2f;
        float i = 0;

        Vector3 sourcePosition = transform.position;
        Vector3 destinationPosition = newHolder.position;

        while (i < 1f)
        {
            i += rate * Time.deltaTime;
            transform.localPosition = Vector3.Lerp(sourcePosition,destinationPosition,i);
            yield return null;
        }

        animateRoutine = null;
        yield return null;
    }


    


}
