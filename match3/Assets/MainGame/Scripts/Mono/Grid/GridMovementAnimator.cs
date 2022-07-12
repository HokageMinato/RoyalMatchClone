using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GridMovementAnimator : Singleton<GridMovementAnimator>
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

    public IEnumerator AnimateCellSwap(MatchExecutionData matchExecutionData) 
    {
        Dictionary<int, List<ElementAnimationData>> swapCellAnimation = AnimateSwapCells(matchExecutionData);
        yield return AnimateMovementRoutine(swapCellAnimation);
    }

    IEnumerator AnimateElementChain(int key, List<ElementAnimationData> animationData, Action<int> onAnimationComplete)
    {
        List<ElementAnimationData> elementAnimationDatas = animationData;
        for (int i = 0; i < elementAnimationDatas.Count; i++)
            yield return elementAnimationDatas[i].Animate();

        onAnimationComplete(key);
    }


    private Dictionary<int, List<ElementAnimationData>> AnimateSwapCells(MatchExecutionData matchExecutionData)
    {
        GridCell firstCell = matchExecutionData.firstCell;
        GridCell secondCell = matchExecutionData.secondCell;

        Element secondElement = secondCell.GetElement();
        Element firstElement = firstCell.GetElement();


        Dictionary<int, List<ElementAnimationData>> initialSwipeAnimationData = new Dictionary<int, List<ElementAnimationData>>()
        {
            {0,GenerateMoveAnimationData(firstElement,secondElement,firstCell,secondCell,matchExecutionData)},
            {1,GenerateMoveAnimationData(secondElement,firstElement,secondCell,firstCell,matchExecutionData)}
        };
        return initialSwipeAnimationData;
    }

    private List<ElementAnimationData> GenerateMoveAnimationData(Element currentElement, Element otherElement, GridCell currentCell, GridCell otherCell, MatchExecutionData data)
    {
        List<ElementAnimationData> elemAnimation = new List<ElementAnimationData>();
        currentCell.SetElement(otherElement);
        elemAnimation.Add(new ElementAnimationData(currentElement, currentCell, otherCell, data, currentCell.HIndex));
        return elemAnimation;
    }

}
