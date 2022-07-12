using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class BoosterSwipeHandler : MonoBehaviour , IMatchHandler
{

    [SerializeField] private MatchGameplayBoosterHitAnimator _boosterHitAnimator;
    [SerializeField] private List<EntityTuple<ElementConfig, GameplayRewardBoosterConfig>> SoloBoosterMap;
    [SerializeField] private List<EntityTuple<EntityTuple<ElementConfig, ElementConfig>,GameplayRewardBoosterConfig>> ComboMap;
    

    private Dictionary <ElementConfig,GameplayRewardBoosterConfig> _soloBoosterMap = new Dictionary<ElementConfig,GameplayRewardBoosterConfig>();
    private Dictionary<EntityTuple<ElementConfig, ElementConfig>, GameplayRewardBoosterConfig> comboBoosterMap = new Dictionary<EntityTuple<ElementConfig, ElementConfig>, GameplayRewardBoosterConfig>();
    private Gridd _grid;

    public void Init()
    {
        foreach (var item in ComboMap)
            comboBoosterMap.Add(item.Key, item.Value);
       
        foreach (var item2 in SoloBoosterMap)
            _soloBoosterMap.Add(item2.Key, item2.Value);
        

        _grid = Gridd.instance;
        _boosterHitAnimator.RegisterOnComplete(OnAnimationCompleted);
    }

    public bool CanHandleSwipe(MatchExecutionData matchExecutionData)
    {
        return IsBoosterSwipe(matchExecutionData.FirstElement.ElementConfig, matchExecutionData.SecondElement.ElementConfig);
    }

    public void HandleSwipe(MatchExecutionData matchExecutionData)
    {
        StartCoroutine(MatchRoutine(matchExecutionData));
    }

    private IEnumerator MatchRoutine(MatchExecutionData executionData) 
    {
        GridMovementAnimator animator = GridMovementAnimator.instance;
        yield return animator.AnimateCellSwap(executionData);
        StartElementDestruction(executionData);
    }

    private void StartElementDestruction(MatchExecutionData matchExecutionData)
    {
        List<EntityTuple<IMatchGameplayRewardBooster, List<Element>>> hitMap = new List<EntityTuple<IMatchGameplayRewardBooster, List<Element>>>();
       
        IMatchGameplayRewardBooster booster = TryGetBoosterAccordingToMatch(matchExecutionData);
        booster.UseBooster(matchExecutionData, _grid);
        

        

        List<IMatchGameplayRewardBooster> chainReactionHits = new List<IMatchGameplayRewardBooster>();
        CheckForOtherBoosterHitsFromPreviousBooster(matchExecutionData, chainReactionHits);
        AddToHitMap(booster, matchExecutionData.boosterHitData.GetAndEmptyHitElements());

        

        //while (chainReactionHits.Count > 0) 
        ////if (chainReactionHits.Count > 0)
        //{
        //    chainReactionHits[0].UseBooster(matchExecutionData, _grid);
        //    _grid.LockDirtyColoumns(matchExecutionData);
        //    CheckForOtherBoosterHitsFromPreviousBooster(matchExecutionData, chainReactionHits);
        //    chainReactionHits.RemoveAt(0);
        //}

        _boosterHitAnimator.ProcessAnimation(matchExecutionData, hitMap);
        void AddToHitMap(IMatchGameplayRewardBooster booster, List<Element> hitElements)
        {
            hitMap.Add(new EntityTuple<IMatchGameplayRewardBooster, List<Element>>()
            {
                Key = booster,
                Value = hitElements
            });
        }
    }

    private void OnAnimationCompleted(MatchExecutionData executionData) 
    {
        StartCoroutine(CollapseColoumns(executionData));
    }

    private IEnumerator CollapseColoumns(MatchExecutionData executionData) 
    {
        yield return MatchFinder.instance.BoosterCollapseRoutine(executionData);
        Gridd.instance.UnlockCells(executionData);
    }

    private void CheckForOtherBoosterHitsFromPreviousBooster(MatchExecutionData matchExecutionData,List<IMatchGameplayRewardBooster> chainReactionHits)
    {
        List<Element> boosterHitElement = matchExecutionData.boosterHitData.AnotherBoosterHitElements;
        if (boosterHitElement.Count == 0)
            return;

        
        for (int i = 0; i < boosterHitElement.Count;)
        {
            chainReactionHits.Add(TryGenerateBooster(boosterHitElement[i].ElementConfig));
            boosterHitElement.RemoveAt(i);
        }

    }



    #region BOOSTER_MAPPING
    private bool IsBoosterSwipe(ElementConfig elem1, ElementConfig elem2)
    {
        Debug.Log($"{ElementFactory.instance.IsBooster(elem1)} || {ElementFactory.instance.IsBooster(elem2)}");
        return ElementFactory.instance.IsBooster(elem1) || ElementFactory.instance.IsBooster(elem2);
    }

    private IMatchGameplayRewardBooster TryGetBoosterAccordingToMatch(MatchExecutionData matchExecutionData)
    {
        IMatchGameplayRewardBooster booster = TryGetComboBooster(matchExecutionData);
        
        if (booster != null)
            return booster;

        return TryGetDefaultBooster(matchExecutionData);
    }

    private IMatchGameplayRewardBooster TryGetComboBooster(MatchExecutionData matchExecutionData) 
    {
        EntityTuple<ElementConfig, ElementConfig> comboConfigTuple = new EntityTuple<ElementConfig, ElementConfig>()
        {
            Key = matchExecutionData.FirstElement.ElementConfig,
            Value = matchExecutionData.SecondElement.ElementConfig
        };

        EntityTuple<ElementConfig, ElementConfig> comboConfigTupleReverse = new EntityTuple<ElementConfig, ElementConfig>()
        {
            Key = comboConfigTuple.Value,
            Value = comboConfigTuple.Key
        };



        if (comboBoosterMap.ContainsKey(comboConfigTuple))
        {
            matchExecutionData.boosterCell = matchExecutionData.firstCell;
            return GenerateGameplayBoosterByConfig(comboBoosterMap[comboConfigTuple]);
        }

        if (comboBoosterMap.ContainsKey(comboConfigTupleReverse))
        {
            matchExecutionData.boosterCell = matchExecutionData.secondCell;
            return GenerateGameplayBoosterByConfig(comboBoosterMap[comboConfigTupleReverse]);
        }



        return null;
    }
    
    private IMatchGameplayRewardBooster TryGetDefaultBooster(MatchExecutionData matchExecutionData) 
    {

        ElementConfig firstElementConfig = matchExecutionData.FirstElement.ElementConfig;
        ElementConfig secondElementConfig = matchExecutionData.SecondElement.ElementConfig;

        Debug.Log($"Element configs recieved {firstElementConfig} {secondElementConfig} Contains Check {_soloBoosterMap.ContainsKey(firstElementConfig)} {_soloBoosterMap.ContainsKey(secondElementConfig)}");

        IMatchGameplayRewardBooster booster = TryGenerateBooster(firstElementConfig);
        
        if (booster != null)
        {
            matchExecutionData.boosterCell = matchExecutionData.firstCell;
            return booster;
        }

        booster =  TryGenerateBooster(secondElementConfig);
        if (booster != null)
        {
            matchExecutionData.boosterCell = matchExecutionData.secondCell;
            return booster;
        }

        return null;
    }

    private IMatchGameplayRewardBooster TryGenerateBooster(ElementConfig elementConfig) 
    {
        if (_soloBoosterMap.ContainsKey(elementConfig))
        {
            return GenerateGameplayBoosterByConfig(_soloBoosterMap[elementConfig]);
        }

        return null;
    }


    private IMatchGameplayRewardBooster GenerateGameplayBoosterByConfig(GameplayRewardBoosterConfig config)
    {
        return GameplayRewardBoosterFactory.instance.GenerateGameplayBoosterByConfig(config);
    }


    #endregion

    
}
