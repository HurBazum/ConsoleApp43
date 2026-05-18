namespace P43.Lib.Messages;
public class CommandMessage : MessageBase
{
    public CommandType Type { get; set; }
}

public enum CommandType
{
    Who,
    Login,
    Exit
}