using System.Collections.Generic;
using UnityEngine;

public class MatchRewardBoosterFactory : Singleton<MatchRewardBoosterFactory>
{
    public List<EntityTuple<ElementConfig, GameObject>> matchRewardBoosters;
    
    private Dictionary<ElementConfig,IMatchRewardBoosterBehaviour> matchRewardBoostersDict = new Dictionary<ElementConfig, IMatchRewardBoosterBehaviour>();


    public void Init() 
    {
        InitilizeLookup();
    }

    private void InitilizeLookup()
    {
        for (int i = 0; i < matchRewardBoosters.Count; i++)
        {
            EntityTuple<ElementConfig,GameObject> item = matchRewardBoosters[i];
            matchRewardBoostersDict.Add(item.Key, item.Value.GetComponent<IMatchRewardBoosterBehaviour>());
        }
    }

    public IMatchRewardBoosterBehaviour GetMatchRewardBooster(ElementConfig elementConfig) 
    { 
        if(matchRewardBoostersDict.ContainsKey(elementConfig))
            return matchRewardBoostersDict[elementConfig];

        return null;
    }

}
