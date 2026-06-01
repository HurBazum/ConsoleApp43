namespace P43.Lib;
public class ReceivedMessageEventArgs : EventArgs
{
    public MessageContext Context { get; init; } = null!;
}