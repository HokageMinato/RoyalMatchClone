using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMatchGameplayRewardBooster 
{
    public void UseBooster(MatchExecutionData executionData, Grid grid, Element placeboElement);
}

