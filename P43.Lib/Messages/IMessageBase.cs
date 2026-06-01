using System.Text.Json.Serialization;

namespace P43.Lib.Messages;

[JsonDerivedType(typeof(PublicMessage), "public")]
[JsonDerivedType(typeof(PrivateMessage), "private")]
[JsonDerivedType(typeof(CommandMessage), "cmd")]
[JsonDerivedType(typeof(SystemMessage), "system" )]
[JsonDerivedType(typeof(LoginRequest), "login_request")]
[JsonDerivedType(typeof(LoginResponse), "login_response")]
[JsonDerivedType(typeof(WhoRequest), "who_request")]
[JsonDerivedType(typeof(CreateRoomRequest), "create_room_request")]
[JsonDerivedType(typeof(JoinRoomRequest), "join_room_request")]
[JsonDerivedType(typeof(ExitRequest), "exit_request")]
[JsonDerivedType(typeof(LeaveRoomRequest), "leave_room_request")]
public interface IMessageBase
{
    Guid SenderId { get; set; }
    string Text { get; set; }
    DateTime SentDate { get; set; }
    string? Login { get; set; }
}