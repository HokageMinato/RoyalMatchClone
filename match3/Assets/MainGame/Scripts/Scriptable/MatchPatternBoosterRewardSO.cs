using UnityEngine;

[CreateAssetMenu(fileName = "MatchPatternBoosterReward" , menuName ="ScriptableObjects/Gameplay/PatternBoosterReward")]
public class MatchPatternBoosterRewardSO : ScriptableObject
{
    public ElementConfig boosterReward;
    public MatchPattern[] matchPatterns;

    

}

