using P43.Lib.Messages;

namespace P43.Lib;
public class MessageContext
{
    public IMessageBase Message { get; set; } = null!;
    public ClientSession Session { get; set; } = null!;
}