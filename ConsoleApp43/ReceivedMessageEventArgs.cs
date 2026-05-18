using ConsoleApp43.Messages;

namespace ConsoleApp43
{
    public class ReceivedMessageEventArgs : EventArgs
    {
        public MessageBase Message { get; init; } = null!;
    }
}