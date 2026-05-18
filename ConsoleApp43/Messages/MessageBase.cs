using System.Text.Json.Serialization;

namespace ConsoleApp43.Messages;

[JsonDerivedType(typeof(PublicMessage), "public")]
[JsonDerivedType(typeof(PrivateMessage), "private")]
[JsonDerivedType(typeof(CommandMessage), "cmd")]
[JsonDerivedType(typeof(SystemMessage), "system" )]
[JsonDerivedType(typeof(LoginRequest), "login_request")]
[JsonDerivedType(typeof(LoginResponse), "login_response")]
[JsonDerivedType(typeof(WhoRequest), "who_request")]
public class MessageBase
{
    public Guid SenderId { get; set; }
    public string Text { get; set; } = string.Empty;
    public DateTime SentDate { get; set; }
    public string? Login { get; set; }


}