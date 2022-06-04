using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GridMovementAnimator : MonoBehaviour
{
    public IEnumerator AnimateMovementRoutine(Dictionary<int,List<ElementAnimationData>> elementFromToPairAnimation)
    {
        WaitForSeconds interAnimationChainDispatchDelay = new WaitForSeconds(.02f);

        foreach (KeyValuePair<int, List<ElementAnimationData>> item in elementFromToPairAnimation)
        {
            StartCoroutine(AnimateElementChain(item.Key, item.Value, (completedId) => { elementFromToPairAnimation.Remove(completedId); }));
            yield return interAnimationChainDispatchDelay;
        }

        while (elementFromToPairAnimation.Count > 0)
            yield return null;

    }

    IEnumerator AnimateElementChain(int key, List<ElementAnimationData> animationData, Action<int> onAnimationComplete)
    {
        yield return null;
        yield return null;

        List<ElementAnimationData> elementAnimationDatas = animationData;
        for (int i = 0; i < elementAnimationDatas.Count; i++)
            yield return elementAnimationDatas[i].Animate();


        yield return null;
        onAnimationComplete(key);
    }
}
