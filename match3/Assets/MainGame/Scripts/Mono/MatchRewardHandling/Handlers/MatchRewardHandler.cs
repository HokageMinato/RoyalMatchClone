using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchRewardHandler : Singleton<MatchRewardHandler>
{
    [SerializeField] private EntityTuple<MatchPattern, ElementConfig>[] rewardData;
    private Dictionary<MatchPattern, ElementConfig> _rewardLookup = new Dictionary<MatchPattern, ElementConfig>();

    public void Init() 
    {
        for (int i = 0; i < rewardData.Length; i++)
        {
            EntityTuple<MatchPattern,ElementConfig> rewardSO = rewardData[i];
            ElementConfig boosterRewardConfig = rewardSO.Value;
            MatchPattern pattern = rewardSO.Key;

            _rewardLookup.Add(pattern, boosterRewardConfig);
        }
    }

    public Element GetElementForPattern(MatchPattern pattern) 
    {
        if(_rewardLookup.ContainsKey(pattern))   
            return ElementFactory.instance.GenerateElementByConfig(_rewardLookup[pattern]);

        return null;
    }

    public void OnMatchPatternBoosterTriggered(Element element) 
    { 
        
    }


    

}
