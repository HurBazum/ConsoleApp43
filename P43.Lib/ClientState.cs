namespace P43.Lib;
public class ClientState
{
    public bool IsAuthorized { get; private set; } = false;

    public void Authorize() => IsAuthorized = true;

}