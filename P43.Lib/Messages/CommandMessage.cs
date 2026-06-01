namespace P43.Lib.Messages;
public class CommandMessage : IMessageBase
{
    public CommandType Type { get; set; }
    public Guid SenderId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentDate { get; set; }
    public string? Login { get; set; }
}

public enum CommandType
{
    Who,
    Login,
    Exit
}