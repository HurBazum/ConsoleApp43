namespace P43.Lib.Messages;

public class PrivateMessage : MessageBase
{
    public string TargetLogin { get; set; } = null!;

    public override string ToString() => $"[{SentDate:t}] {Login} to {TargetLogin}: {Text}";
}