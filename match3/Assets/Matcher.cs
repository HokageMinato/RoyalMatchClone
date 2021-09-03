using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

public class Matcher : Singleton<Matcher>
{

    public int matchCount;
    
    public void StartChecking()
    {
        StartCoroutine(CheckRoutine());
        
    }

    private IEnumerator CheckRoutine()
    {
        
        yield return null;

        while (matchCount > 0)
        {
            Grid.instance.Destruct();
            while (Grid.instance.IsAnimating)
            {
                yield return null;
            }

            
            yield return new WaitForSeconds(0.2f);
            matchCount--;
        }

        yield return new WaitForSeconds(2f);
        matchCount = 5;
    }


}
