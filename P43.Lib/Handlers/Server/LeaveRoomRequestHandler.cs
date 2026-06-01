using P43.Lib.Managers.Interfaces;
using P43.Lib.Messages;

namespace P43.Lib.Handlers.Server;
public class LeaveRoomRequestHandler(ISessionManager sessionManager, IRoomManager roomManager) : IMessageHandler<LeaveRoomRequest>
{
    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IRoomManager _roomManager = roomManager;

    public Task HandleAsync(LeaveRoomRequest message, ClientSession session)
    {
        if(session.Room == null)
        {
            session.GetMessage(new SystemMessage()
            {
                SentDate = DateTime.UtcNow,
                SenderId = session.SessionId,
                Text = $"You are not in any room. Please join a room before leaving it."
            });

            return Task.CompletedTask;
        }

        var room = session.Room;
        session.Room = null;

        session.GetMessage(new SystemMessage()
        {
            SentDate = DateTime.UtcNow,
            SenderId = session.SessionId,
            Text = $"You have left room '{room.Name}'"
        });

        room.RemoveMember(session.SessionId);

        if(room.MemberCount == 0)
        {
            _roomManager.RemoveRoom(room.Name);
        }
        else
        {
            NotifyAsync(new SystemMessage()
            {
                SentDate = DateTime.UtcNow,
                SenderId = session.SessionId,
                Text = $"User '{session.Login}' has left the room"
            }, room);
        }

        return Task.CompletedTask;
    }

    private Task NotifyAsync(SystemMessage message, Room room)
    {
        var sessions = _sessionManager.GetAllSessions(x => x.Room == room);
        foreach(var s in sessions)
        {
            s.GetMessage(message);
        }

        return Task.CompletedTask;
    }
}