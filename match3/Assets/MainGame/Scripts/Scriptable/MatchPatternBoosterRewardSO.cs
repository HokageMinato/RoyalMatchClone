using UnityEngine;

[CreateAssetMenu(fileName = "MatchPatternBoosterReward" , menuName ="ScriptableObjects/Gameplay/PatternBoosterReward")]
public class MatchPatternBoosterRewardSO : ScriptableObject
{
    public MatchPattern matchPattern;
    public ElementConfig boosterReward;
}

