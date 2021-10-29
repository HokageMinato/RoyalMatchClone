using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Element : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public const float SWIPE_ANIM_TIME = 0.5f;

    public GameLayer renderLayer;
    public ElementType elementType;
    
    public void Start()
    {
        spriteRenderer.sortingOrder = (int)renderLayer;
    }

    public bool IsSame(Element other)
   {
      if (other == null)
         return false;
      
      return elementType==other.elementType;
   }

   public void SetHolder(GridCell newHolder)
   {
      transform.SetParent(newHolder.transform);
      StartCoroutine(ShiftRoutine(newHolder.executionData));
   }

    public void SetHolderWithPath(GridCell newHolder,List<GridCell> travelPath) {
        transform.SetParent(newHolder.transform);
        StartCoroutine(PathRoutine(newHolder.executionData,travelPath));
    }

    private IEnumerator ShiftRoutine(MatchExecutionData executionData)
    {
        Debug.Log($"edata null {executionData == null}");
        executionData.movingElements.Add(this);
        float rate = 1f / SWIPE_ANIM_TIME;
        float i = 0;
        Vector3 sourcePosition = transform.localPosition;
        Vector3 pseudoZero = new Vector3(0.001f, 0.001f, 0.01f);

        while (i <= 1f)
        {
            i += rate * Time.deltaTime;
            transform.localPosition = Vector3.Lerp(sourcePosition, pseudoZero, i);
            yield return null;
        }
       executionData.movingElements.Remove(this);
    }
    private IEnumerator PathRoutine(MatchExecutionData executionData,List<GridCell> path)
    {

       executionData.movingElements.Add(this);
        float rate = 1f / SWIPE_ANIM_TIME;
        float i = 0;
        Vector3 sourcePosition = transform.localPosition;
        Vector3 pseudoZero = new Vector3(0.001f, 0.001f, 0.01f);

        while (path.Count > 0)
        {
            int lastIndex = path.Count - 1;
            pseudoZero = transform.InverseTransformPoint(path[lastIndex].transform.position);
            while (i <= 1f)
            {
                i += rate * Time.deltaTime;
                transform.localPosition = Vector3.Lerp(sourcePosition, pseudoZero, i);
                yield return null;
            }
            path.RemoveAt(lastIndex);
        }
        executionData.movingElements.Remove(this);
      }

   
  

   public enum ElementType
   {
      Type1,
      Type2,
      Type3,
      Type4
   }
  
}
