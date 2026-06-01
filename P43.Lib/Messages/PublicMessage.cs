namespace P43.Lib.Messages;

public class PublicMessage : IMessageBase
{
    public Guid SenderId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentDate { get; set; }
    public string? Login { get; set; }
    public override string ToString()
    {
        if(!string.IsNullOrEmpty(Login))
        {
            return $"[{SentDate:t}] {Login}: {Text}";
        }
        else
        {
            return $"[{SentDate:t}] {SenderId.ToString()[..8]}: {Text}";
        }
    }
}