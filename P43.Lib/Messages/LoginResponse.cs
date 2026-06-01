namespace P43.Lib.Messages;

public class LoginResponse : IMessageBase
{
    public Guid SenderId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentDate { get; set; }
    public string? Login { get; set; }
    public bool Success { get; set; }
}