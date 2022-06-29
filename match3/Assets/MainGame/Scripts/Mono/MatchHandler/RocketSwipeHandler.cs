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
    
    private Element firstElement;
    private Element secondElement;

    private Element _activePlaceboElement;

    private bool _isInited = false;
    private Grid _grid;

    public void Init()
    {
        if (_isInited)
            return;

        foreach (var item in BoosterMap)
            _boosterMap .Add(item.Key, GenerateActivePrefab(item.Value));


        foreach (var item2 in DefaultBoosterMap)
            _defaultBoosterMap.Add(item2.Key, GenerateActivePrefab(item2.Value));
        

        _isInited = true;
        _grid = Grid.instance;
    }

    private IRocketBooster GenerateActivePrefab(MonoBehaviour item)
    {
        MonoBehaviour ob = Instantiate(item,transform);
        ob.gameObject.SetActive(false);
        return ob as IRocketBooster;
    }

    public void OnSwipeRecieved(MatchExecutionData matchExecutionData)
    {
        Debug.Log("Handling RocketSwipe");
        PopulateElementsData(matchExecutionData);
        InstantiateBooster(GetRocketBooster()).UseBooster(matchExecutionData,_grid,_activePlaceboElement); 
    }

    private IRocketBooster InstantiateBooster(IRocketBooster booster)
    {
        MonoBehaviour mono = Instantiate((MonoBehaviour)booster, _grid.GetLayerTransformParent(RenderLayer.ElementLayer));
        mono.gameObject.SetActive(true);
        return  mono as IRocketBooster; 
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
        firstElement = matchExecutionData.firstCell.ReadElement();
        secondElement = matchExecutionData.secondCell.ReadElement();
    }

    private IRocketBooster GetPairedBooster() 
    {
        ElementConfig firstElementConfig = firstElement.ElementConfig;
        ElementConfig secondElementConfig = secondElement.ElementConfig;

        if (_boosterMap.ContainsKey(firstElementConfig))
        {
            _activePlaceboElement = firstElement;
            return _boosterMap[firstElementConfig];
        }

        if (_boosterMap.ContainsKey(secondElementConfig))
        {
            _activePlaceboElement = secondElement;
            return _boosterMap[secondElementConfig];
        }

        return null;
    }
    
    private IRocketBooster GetDefaultBooster() 
    {

        if (_defaultBoosterMap.ContainsKey(firstElement.ElementConfig))
            return _defaultBoosterMap[firstElement.ElementConfig];

        if(_defaultBoosterMap.ContainsKey(secondElement.ElementConfig))
            return _defaultBoosterMap[secondElement.ElementConfig];

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
