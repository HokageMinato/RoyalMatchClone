using System.Collections.Generic;
using UnityEngine;

public class MatchRewardBoosterFactory : Singleton<MatchRewardBoosterFactory>
{
    public List<EntityTuple<ElementConfig, GameObject>> matchRewardBoosters;
    
    private Dictionary<ElementConfig,IMatchRewardBooster> matchRewardBoostersDict = new Dictionary<ElementConfig, IMatchRewardBooster>();


    public void Init() 
    {
        InitilizeLookup();
    }

    private void InitilizeLookup()
    {
        for (int i = 0; i < matchRewardBoosters.Count; i++)
        {
            EntityTuple<ElementConfig,GameObject> item = matchRewardBoosters[i];
            matchRewardBoostersDict.Add(item.Key, item.Value.GetComponent<IMatchRewardBooster>());
        }
    }

    public IMatchRewardBooster GetMatchRewardBooster(ElementConfig elementConfig) 
    { 
        if(matchRewardBoostersDict.ContainsKey(elementConfig))
            return matchRewardBoostersDict[elementConfig];

        return null;
    }

}
