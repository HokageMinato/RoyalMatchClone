using UnityEngine;
using System.Collections.Generic;

public class RocketSwipeHandler : MonoBehaviour , IMatchHandler
{
    [SerializeField]
    [Header("Key = SecondElementWithRocket, Value = AppBoosterHandler")] 
    private List<EntityTuple<ElementConfig, MonoBehaviour>> BoosterMap;

    [SerializeField]
    [Header("Key = DefaultSimpleRocketElements, Value = AppBoosterHandler")]
    private List<EntityTuple<ElementConfig, MonoBehaviour>> DefaultBoosterMap;
    
    private Dictionary <ElementConfig,IRocketBooster> _boosterMap = new Dictionary<ElementConfig,IRocketBooster>();
    private Dictionary <ElementConfig,IRocketBooster> _defaultBoosterMap = new Dictionary<ElementConfig,IRocketBooster>();
    
    private ElementConfig firstElementDetails;
    private ElementConfig secondElementDetails;

    private bool _isInited = false;

    public void Init()
    {
        if (_isInited)
            return;

        foreach (var item in BoosterMap)
            _boosterMap .Add(item.Key, Instantiate(item.Value) as IRocketBooster);


        foreach (var item2 in DefaultBoosterMap)
        {
            Debug.Log($"{item2.Key} -- {_defaultBoosterMap.ContainsKey(item2.Key)}");
            _defaultBoosterMap.Add(item2.Key, Instantiate(item2.Value) as IRocketBooster);
        }
        
        _isInited = true;
        
    }

    
    public void OnSwipeRecieved(MatchExecutionData matchExecutionData)
    {
        Debug.Log("Handling RocketSwipe");
        
        PopulateElementsData(matchExecutionData);
        IRocketBooster booster = GetRocketBooster();
        booster = InstantiateBooster(booster);
        booster.UseBooster(matchExecutionData,Grid.instance);
    }

    private IRocketBooster InstantiateBooster(IRocketBooster booster) 
    {
        Object unityOb = (Object)booster;
        Instantiate(unityOb);
        return unityOb as IRocketBooster;
    }

    private IRocketBooster GetRocketBooster()
    {
        IRocketBooster booster = GetPairedBooster();
        
        if (booster != null)
            return booster;

        return GetDefaultBooster();
    }

    private void PopulateElementsData(MatchExecutionData matchExecutionData)
    {
        firstElementDetails = matchExecutionData.firstCell.ReadElement().ElementConfig;
        secondElementDetails = matchExecutionData.secondCell.ReadElement().ElementConfig;
    }

    private IRocketBooster GetPairedBooster() 
    {
        if (_boosterMap.ContainsKey(firstElementDetails))
            return _boosterMap[firstElementDetails];

        if(_boosterMap.ContainsKey(secondElementDetails))
            return _boosterMap[secondElementDetails];

        return null;
    }
    
    private IRocketBooster GetDefaultBooster() 
    {
        if (_defaultBoosterMap.ContainsKey(firstElementDetails))
            return _defaultBoosterMap[firstElementDetails];

        if(_defaultBoosterMap.ContainsKey(secondElementDetails))
            return _defaultBoosterMap[secondElementDetails];

        return null;
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        foreach (var item in BoosterMap)
        {
            if (item.Value != null && !(item.Value is IRocketBooster))
            {
                item.Value = null;
                Debug.LogError($"Only implementations of {typeof(IRocketBooster)} are allowed");
            }
        }

        
    }
#endif

}
