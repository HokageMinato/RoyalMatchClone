using UnityEngine;
using System.Collections.Generic;

public class BoosterSwipeHandler : MonoBehaviour , IMatchHandler
{
    
    [SerializeField] private List<EntityTuple<ElementConfig, GameplayRewardBoosterConfig>> SoloBoosterMap;
    [SerializeField] private List<EntityTuple<EntityTuple<ElementConfig, ElementConfig>,GameplayRewardBoosterConfig>> ComboMap;

    private Dictionary <ElementConfig,GameplayRewardBoosterConfig> _boosterMap = new Dictionary<ElementConfig,GameplayRewardBoosterConfig>();
    private Dictionary<EntityTuple<ElementConfig, ElementConfig>, GameplayRewardBoosterConfig> _defaultBoosterMap = new Dictionary<EntityTuple<ElementConfig, ElementConfig>, GameplayRewardBoosterConfig>();

    private Element firstElement;
    private Element secondElement;
    private Element _activePlaceboElement;

    private Grid _grid;

    public void Init()
    {

        foreach (var item in ComboMap)
        {
            if (_defaultBoosterMap.ContainsKey(item.Key))
                Debug.LogError($"{item.Key} Duplicate");

            _defaultBoosterMap.Add(item.Key, item.Value);
        }
        

        foreach (var item2 in SoloBoosterMap)
            _boosterMap.Add(item2.Key, item2.Value);
        

        _grid = Grid.instance;
    }


    public void OnSwipeRecieved(MatchExecutionData matchExecutionData)
    {
        Debug.Log("Handling RocketSwipe");
        PopulateElementsData(matchExecutionData);
        GetRocketBoosterAccordingToMatch().UseBooster(matchExecutionData,_grid,_activePlaceboElement); 
    }

    private IMatchGameplayRewardBooster GetRocketBoosterAccordingToMatch()
    {
        IMatchGameplayRewardBooster booster = TryGetComboBooster();
        
        if (booster != null)
            return booster;

        return GetDefaultBooster();
    }

    private void PopulateElementsData(MatchExecutionData matchExecutionData)
    {
        firstElement = matchExecutionData.firstCell.ReadElement();
        secondElement = matchExecutionData.secondCell.ReadElement();
    }

    private IMatchGameplayRewardBooster TryGetComboBooster() 
    {
        EntityTuple<ElementConfig, ElementConfig> comboConfigTuple = new EntityTuple<ElementConfig, ElementConfig>()
        {
            Key = firstElement.ElementConfig,
            Value = secondElement.ElementConfig
        };

        EntityTuple<ElementConfig, ElementConfig> comboConfigTupleReverse = new EntityTuple<ElementConfig, ElementConfig>()
        {
            Key = firstElement.ElementConfig,
            Value = secondElement.ElementConfig
        };



        if (_defaultBoosterMap.ContainsKey(comboConfigTuple))
        {
            _activePlaceboElement = firstElement;
            return GenerateGameplayBooster(_defaultBoosterMap[comboConfigTuple]);
        }

        if (_defaultBoosterMap.ContainsKey(comboConfigTupleReverse))
        {
            _activePlaceboElement = secondElement;
            return GenerateGameplayBooster(_defaultBoosterMap[comboConfigTupleReverse]);
        }



        return null;
    }
    
    private IMatchGameplayRewardBooster GetDefaultBooster() 
    {

        ElementConfig firstElementConfig = firstElement.ElementConfig;
        ElementConfig secondElementConfig = secondElement.ElementConfig;

        if (_boosterMap.ContainsKey(firstElementConfig))
            return GenerateGameplayBooster(_boosterMap[firstElementConfig]);

        if (_boosterMap.ContainsKey(secondElementConfig))
            return GenerateGameplayBooster(_boosterMap[secondElementConfig]);

        return null;
    }


    private IMatchGameplayRewardBooster GenerateGameplayBooster(GameplayRewardBoosterConfig config)
    {
        return GameplayRewardBoosterFactory.instance.GenerateGameplayBoosterByConfig(config);
    }

}
