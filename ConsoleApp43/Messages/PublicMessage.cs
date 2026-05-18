namespace ConsoleApp43.Messages;
public class PublicMessage : MessageBase
{
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