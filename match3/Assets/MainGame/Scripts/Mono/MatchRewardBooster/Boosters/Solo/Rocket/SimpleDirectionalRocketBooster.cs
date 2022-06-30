using System;
using UnityEngine;

public class SimpleDirectionalRocketBooster : MonoBehaviour, IMatchGameplayRewardBooster
{
    private MatchExecutionData _executionData;
    private Grid _grid;
    private Element _placeboElement;

    public void UseBooster(MatchExecutionData executionData, Grid grid, Element placeboElement)
    {
        Debug.Log("From normal Rocket Booster");
        PopulateParameters(executionData,grid,placeboElement);
        SetSelfAtPlaceboPosition();
    }

    private void PopulateParameters(MatchExecutionData executionData, Grid grid, Element placeboElement)
    {
        MatchExecutionData _executionData = executionData;
        Grid _grid = grid;
        Element _placeboElement = placeboElement;
    }

    private void SetSelfAtPlaceboPosition()
    {
        transform.position = _placeboElement.transform.position;
        _placeboElement.GetComponentInChildren<SpriteRenderer>(true).color = Color.red;

    }
}
