
public interface IMatchHandler 
{
    public void Init();
    public void HandleSwipe(MatchExecutionData matchExecutionData);

    public bool CanHandleSwipe(MatchExecutionData matchExecutionData);
}
