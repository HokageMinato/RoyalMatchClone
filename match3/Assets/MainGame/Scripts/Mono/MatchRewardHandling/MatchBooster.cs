using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMatchRewardBooster
{
    public abstract void OnSwiped(MatchExecutionData executionData);
}
