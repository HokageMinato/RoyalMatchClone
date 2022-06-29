using UnityEngine;

public class SimpleHorizontalRocketBooster : MonoBehaviour, IRocketBooster
{
    

    public void UseBooster(MatchExecutionData executionData, Grid grid)
    {
        Debug.Log("From normal Rocket Booster");
    }
}
