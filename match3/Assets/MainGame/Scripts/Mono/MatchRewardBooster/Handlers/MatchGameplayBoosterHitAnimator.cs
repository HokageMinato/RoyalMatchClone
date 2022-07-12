using System;
using System.Collections.Generic;
using UnityEngine;

public class MatchGameplayBoosterHitAnimator : MonoBehaviour
{
    private Action<MatchExecutionData> _onComplete;

    public void RegisterOnComplete(Action<MatchExecutionData> onComplete) 
    { 
        _onComplete = onComplete;
    }

    public void ProcessAnimation(MatchExecutionData data,List<EntityTuple<IMatchGameplayRewardBooster, List<Element>>> hitMap) 
    {
        //MANAGE ANIMATION HERE.
        //ALL BOOSTER AND THEIR RESPECTIVE TO BE DESTORYED ELEMENTS ARE MAPPED HERE.
        //THIS HITMAP INSTANCE IS FOR EACH INDIVIDUAL MATCH LOOP

        //INVOKING ONCOMPLETE WILL BEGIN NORMAL MATCHING SEQUENCE.
        TempDestroyBoosterHitElements(hitMap);
        InvokeOnComplete(data);
    }

    private void TempDestroyBoosterHitElements(List<EntityTuple<IMatchGameplayRewardBooster, List<Element>>> hitMap)
    {
        foreach (var item in hitMap)
        {
            DestroyHitElements(item);
        }

    }

    private void DestroyHitElements(EntityTuple<IMatchGameplayRewardBooster, List<Element>> hitData) 
    {
        Debug.Log($"Destroying for {hitData.Key}");
        DestroyImmediate(((MonoBehaviour)hitData.Key).gameObject);
       
        List<Element> hitElements = hitData.Value;
        foreach (var element in hitElements) 
        { 
            element.DestroyElement();
        }
    }

    private void InvokeOnComplete(MatchExecutionData data) 
    { 
        _onComplete(data);
    }

}
