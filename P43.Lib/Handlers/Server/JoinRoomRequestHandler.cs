using P43.Lib.Messages;
using P43.Lib.Managers.Interfaces;

namespace P43.Lib.Handlers.Server;
public class JoinRoomRequestHandler(IRoomManager roomManager, ISessionManager sessionManager) : IMessageHandler<JoinRoomRequest>
{
    private readonly IRoomManager _roomManager = roomManager;
    private readonly ISessionManager _sessionManager = sessionManager;

    public Task HandleAsync(JoinRoomRequest joinRoomRequest, ClientSession session)
    {
        if(session.Room != null)
        {
            session.GetMessage(new SystemMessage()
            {
                SentDate = DateTime.UtcNow,
                SenderId = session.SessionId,
                Text = $"You are already in room '{session.Room.Name}'. Please leave it before joining another one."
            });

            return Task.CompletedTask;
        }

        var targetRoom = _roomManager.GetByName(joinRoomRequest.Text);

        if(targetRoom == null)
        {
            session.GetMessage(new SystemMessage()
            {
                SentDate = DateTime.UtcNow,
                SenderId = session.SessionId,
                Text = $"Room '{joinRoomRequest.Text}' does not exist. Please check the name and try again."
            });

            return Task.CompletedTask;
        }

        session.Room = targetRoom;
        targetRoom.AddMember(session.SessionId);

        session.GetMessage(new SystemMessage()
        {
            SentDate = DateTime.UtcNow,
            SenderId = session.SessionId,
            Text = $"You have joined room '{targetRoom.Name}'"
        });

        NotifyAsync(new SystemMessage()
        {
            SentDate = DateTime.UtcNow,
            SenderId = session.SessionId,
            Text = $"User '{session.Login}' has joined the room"
        }, targetRoom, session.SessionId);


        return Task.CompletedTask;
    }

    private Task NotifyAsync(SystemMessage message, Room room, Guid excludeSessionId)
    {
        var sessions = _sessionManager.GetAllSessions(x => x.Room == room && x.SessionId != excludeSessionId);
        foreach(var s in sessions)
        {
            s.GetMessage(message);
        }
        return Task.CompletedTask;
    }
}