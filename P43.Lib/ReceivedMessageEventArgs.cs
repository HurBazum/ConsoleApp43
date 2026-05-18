using P43.Lib.Messages;

namespace P43.Lib;
public class ReceivedMessageEventArgs : EventArgs
{
    public MessageBase Message { get; init; } = null!;
}