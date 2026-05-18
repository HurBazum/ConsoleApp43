namespace ConsoleApp43.Messages;
public class SystemMessage : MessageBase
{
    public override string ToString() => $"[{SentDate:t}] System: {Text}";
}