using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

public class Matcher : Singleton<Matcher>
{
    [SerializeField] private MatchPattern patterns;
   
    public void StartChecking()
    {
        StartCoroutine(CheckRoutine());
        
    }

    private IEnumerator CheckRoutine()
    {

        int matchCount = 0;
        Grid grid = Grid.instance;
        
        yield return null;

        while (matchCount > 0)
        {
        
            
            
        
            while (grid.IsAnimating)
            {
                yield return null;
            }


            yield return new WaitForSeconds(0.2f);
            matchCount--;
        }

        yield return new WaitForSeconds(2f);
       
    }

    [ContextMenu("Check")]
    public void FindMatches()
    {
        Grid grid = Grid.instance;
        List<Element> matches = new List<Element>();

       

    }
    

}
