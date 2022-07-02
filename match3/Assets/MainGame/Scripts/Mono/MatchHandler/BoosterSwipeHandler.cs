using UnityEngine;
using System.Collections.Generic;

public class BoosterSwipeHandler : MonoBehaviour , IMatchHandler
{
    
    [SerializeField] private List<EntityTuple<ElementConfig, GameplayRewardBoosterConfig>> SoloBoosterMap;
    [SerializeField] private List<EntityTuple<EntityTuple<ElementConfig, ElementConfig>,GameplayRewardBoosterConfig>> ComboMap;

    private Dictionary <ElementConfig,GameplayRewardBoosterConfig> _soloBoosterMap = new Dictionary<ElementConfig,GameplayRewardBoosterConfig>();
    private Dictionary<EntityTuple<ElementConfig, ElementConfig>, GameplayRewardBoosterConfig> comboBoosterMap = new Dictionary<EntityTuple<ElementConfig, ElementConfig>, GameplayRewardBoosterConfig>();

    private Grid _grid;

    public void Init()
    {

        foreach (var item in ComboMap)
        {
            comboBoosterMap.Add(item.Key, item.Value);
        }
        

        foreach (var item2 in SoloBoosterMap)
            _soloBoosterMap.Add(item2.Key, item2.Value);
        

        _grid = Grid.instance;
    }


    public void OnSwipeRecieved(MatchExecutionData matchExecutionData)
    {
        GetRocketBoosterAccordingToMatch(matchExecutionData).UseBooster(matchExecutionData,_grid); 
    }

    private IMatchGameplayRewardBooster GetRocketBoosterAccordingToMatch(MatchExecutionData matchExecutionData)
    {
        IMatchGameplayRewardBooster booster = TryGetComboBooster(matchExecutionData);
        
        if (booster != null)
            return booster;

        return GetDefaultBooster(matchExecutionData);
    }

    private void PopulateElementsData(MatchExecutionData matchExecutionData)
    {
    
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
            return GenerateGameplayBooster(comboBoosterMap[comboConfigTuple]);
        }

        if (comboBoosterMap.ContainsKey(comboConfigTupleReverse))
        {
            matchExecutionData.boosterCell = matchExecutionData.secondCell;
            return GenerateGameplayBooster(comboBoosterMap[comboConfigTupleReverse]);
        }



        return null;
    }
    
    private IMatchGameplayRewardBooster GetDefaultBooster(MatchExecutionData matchExecutionData) 
    {

        ElementConfig firstElementConfig = matchExecutionData.FirstElement.ElementConfig;
        ElementConfig secondElementConfig = matchExecutionData.SecondElement.ElementConfig;

        Debug.Log($"Element configs recieved {firstElementConfig} {secondElementConfig} Contains Check {_soloBoosterMap.ContainsKey(firstElementConfig)} {_soloBoosterMap.ContainsKey(secondElementConfig)}");

        string dict = string.Empty;

        foreach (var item in _soloBoosterMap)
            dict += item.Key.ToString() + "\n";

        Debug.Log($"dict: {dict}");



        if (_soloBoosterMap.ContainsKey(firstElementConfig))
        {
            matchExecutionData.boosterCell = matchExecutionData.firstCell;
            return GenerateGameplayBooster(_soloBoosterMap[firstElementConfig]);
        }

        if (_soloBoosterMap.ContainsKey(secondElementConfig))
        {
            matchExecutionData.boosterCell = matchExecutionData.secondCell;
            return GenerateGameplayBooster(_soloBoosterMap[secondElementConfig]);
        }

        return null;
    }


    private IMatchGameplayRewardBooster GenerateGameplayBooster(GameplayRewardBoosterConfig config)
    {
        return GameplayRewardBoosterFactory.instance.GenerateGameplayBoosterByConfig(config);
    }

    

}
