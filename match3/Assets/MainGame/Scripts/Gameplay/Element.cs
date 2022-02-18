using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Element : MonoBehaviour
{

    [SerializeField] ElementConfig elementConfig;
    IEnumerator animateRoutine;
    bool destoryPostAnimation;

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
        if (animateRoutine == null)
            DestroyImmediate(gameObject);
        else
            destoryPostAnimation = true;
    }
   
    public IEnumerator AnimateToCellRoutine(GridCell newHolder)
    {

        MatchExecutionData executionData = newHolder.executionData;

        if (executionData == null)
            Debug.Log($"null at cell {newHolder.gameObject.name}");

        executionData.movingElements.Add(this);
       
        while (newHolder.lockedInAnimation)
            yield return null;

       
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
        newHolder.lockedInAnimation = false;
        animateRoutine = null;
        
        if (destoryPostAnimation) 
        {
            DestroyElement();
        }
    }





}
