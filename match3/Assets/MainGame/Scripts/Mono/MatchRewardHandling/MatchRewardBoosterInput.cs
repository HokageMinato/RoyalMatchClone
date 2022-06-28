using UnityEngine;
using UnityEngine.EventSystems;

public class MatchBoosterInput:MonoBehaviour , IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        OnTapped(GenerateExecutionData());
    }

    public void OnSwiped(MatchExecutionData executionData) 
    {
        GetComponent<IMatchRewardBoosterBehaviour>().OnSwiped(executionData);
    }

    public void OnTapped(MatchExecutionData executionData) 
    {
        GetComponent<IMatchRewardBoosterBehaviour>().OnTapped(executionData);
    }

    

    private MatchExecutionData GenerateExecutionData()
    {
        return MatchExecutionData.GetDefaultExecutionData();
    }

}
