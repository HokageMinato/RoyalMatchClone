
using System;

public interface IMatchRewardBoosterBehaviour 
{
    public void OnTapped(MatchExecutionData executionData);
    public void OnSwiped(MatchExecutionData executionData);
    public void RegisterOnComplete(Action onComplete);
}
