namespace P43.Lib.Messages;

public class SystemMessage : MessageBase
{
    public override string ToString() => $"[{SentDate:t}] System: {Text}";
}