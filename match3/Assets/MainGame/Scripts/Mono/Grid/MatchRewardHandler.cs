using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchRewardHandler : Singleton<MatchRewardHandler>
{
    [SerializeField] private MatchPatternBoosterRewardSO[] rewardsData;
    //[SerializeField] private SerizTuple<MatchPattern,>
    
    private Dictionary<MatchPattern, ElementConfig> _rewardLookup = new Dictionary<MatchPattern, ElementConfig>();
    private Dictionary<ElementConfig,BaseMatchBooster> _matchBoosterLookup = new Dictionary<ElementConfig, BaseMatchBooster>();

    public void Init() 
    {
        for (int i = 0; i < rewardsData.Length; i++)
        {
            MatchPatternBoosterRewardSO rewardSO = rewardsData[i];
            ElementConfig boosterRewardConfig = rewardSO.boosterReward;
            MatchPattern[] patterns = rewardSO.matchPatterns;

            for (int j = 0; j < patterns.Length; j++)
            {
                _rewardLookup.Add(patterns[j], boosterRewardConfig);
            }
        }
    }

    

    public Element TryGeneratePatternReward(MatchPattern pattern) 
    {
        if(_rewardLookup.ContainsKey(pattern))   
            return ElementFactory.instance.GenerateElementByConfig(_rewardLookup[pattern]);

        return null;
    }

    public void OnElementBoosterTriggered(Element element) 
    { 
        
    }

}
