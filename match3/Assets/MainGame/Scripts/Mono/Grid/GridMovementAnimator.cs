using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GridMovementAnimator : MonoBehaviour
{
    public IEnumerator AnimateMovementRoutine(Dictionary<int,List<ElementAnimationData>> elementFromToPairAnimation)
    {
        WaitForSeconds interAnimationChainDispatchDelay = new WaitForSeconds(.02f);
        Dictionary<int,List<ElementAnimationData>> copy = new Dictionary<int, List<ElementAnimationData>>(elementFromToPairAnimation);

        foreach (KeyValuePair<int, List<ElementAnimationData>> item in elementFromToPairAnimation)
        {
            if (copy.ContainsKey(item.Key))
            {
                StartCoroutine(AnimateElementChain(item.Key, item.Value, (completedId) => { copy.Remove(completedId); }));
                yield return interAnimationChainDispatchDelay;
            }
        }


        while (copy.Count > 0)
            yield return null;

    }

    IEnumerator AnimateElementChain(int key, List<ElementAnimationData> animationData, Action<int> onAnimationComplete)
    {
        List<ElementAnimationData> elementAnimationDatas = animationData;
        for (int i = 0; i < elementAnimationDatas.Count; i++)
            yield return elementAnimationDatas[i].Animate();

        onAnimationComplete(key);
    }
}
