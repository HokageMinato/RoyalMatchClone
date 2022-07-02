using System;
using UnityEngine;

public class SimpleDirectionalRocketBooster : MonoBehaviour, IMatchGameplayRewardBooster
{
    private MatchExecutionData _executionData;
    private GridCell _targetCell;


    public void UseBooster(MatchExecutionData executionData, Grid grid)
    {
        Debug.Log("From normal Rocket Booster");
        PopulateParameters(executionData,grid);
        SetSelfAtPlaceboPosition();
    }

    private void PopulateParameters(MatchExecutionData executionData, Grid grid)
    {
        _executionData = executionData;
        _targetCell = executionData.boosterCell;
    }

    private void SetSelfAtPlaceboPosition()
    {
        transform.position = _targetCell.transform.position;
        _targetCell.ReadElement().GetComponent<SpriteRenderer>().color = Color.red;
        Debug.Log("Exec placebo");
    }
}
