namespace P43.Lib.Messages;

public class PrivateMessage : IMessageBase
{
    public Guid SenderId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentDate { get; set; }
    public string? Login { get; set; }
    public string TargetLogin { get; set; } = null!;

    public override string ToString() => $"[{SentDate:t}] from {Login}: {Text}";
}