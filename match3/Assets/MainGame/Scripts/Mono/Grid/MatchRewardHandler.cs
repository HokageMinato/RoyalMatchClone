using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchRewardHandler : Singleton<MatchRewardHandler>
{
    [SerializeField] private MatchPatternBoosterRewardSO[] rewardsData;
    private Dictionary<MatchPattern, ElementConfig> rewardLookup = new Dictionary<MatchPattern, ElementConfig>();

    public void Init() 
    {
        for (int i = 0; i < rewardsData.Length; i++)
            rewardLookup.Add(rewardsData[i].matchPattern,rewardsData[i].boosterReward);
    }

    public ElementConfig FetchRewardConfig(MatchPattern pattern) 
    {
        return rewardLookup[pattern];
    }

}
