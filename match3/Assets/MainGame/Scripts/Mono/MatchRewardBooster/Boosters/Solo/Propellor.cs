using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propellor : MonoBehaviour , IMatchGameplayRewardBooster
{

    private MatchExecutionData _executionData;
    private Gridd _grid;
    private List<GridCell> _cellsToBeHit = new List<GridCell>();

    public void UseBooster(MatchExecutionData executionData, Gridd grid)
    {

    }

    
}
