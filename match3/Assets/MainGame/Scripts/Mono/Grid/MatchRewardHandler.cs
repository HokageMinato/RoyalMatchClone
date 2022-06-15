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
        {
            MatchPatternBoosterRewardSO rewardSO = rewardsData[i];
            ElementConfig boosterRewardConfig = rewardSO.boosterReward;
            MatchPattern[] patterns = rewardSO.matchPatterns;

            for (int j = 0; j < patterns.Length; j++)
            {
                rewardLookup.Add(patterns[j], boosterRewardConfig);
            }
        }
    }

    public Element GenerateBoosterElements(MatchData matchData) 
    {
        ElementConfig config = matchData.BoosterReward;
        GridCell targetCell = matchData.TargetSpawningCell;
        Element rewardElement = ElementFactory.instance.GenerateElementByConfig(config);
        targetCell.SetElement(rewardElement);
        rewardElement.transform.localPosition = targetCell.transform.localPosition;
        return rewardElement;
    }

    public ElementConfig FetchRewardConfig(MatchPattern pattern) 
    {
        if(rewardLookup.ContainsKey(pattern))   
            return rewardLookup[pattern];

        return null;
    }

}
