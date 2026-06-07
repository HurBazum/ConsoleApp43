using P43.Lib.Messages;
using P43.Lib.Managers.Interfaces;

namespace P43.Lib.Handlers.Server;

public class WhoRequestHandler(ISessionManager sessionManager) : IRequestHandler<WhoRequest>
{
    private readonly ISessionManager _sessionManager = sessionManager;

    public Task HandleAsync(WhoRequest message, ClientSession session)
    {
        var room = session.Room;
        
        if(room == null)
        {
            var count = _sessionManager.Count();

            session.GetMessage(new SystemMessage
            {
                SenderId = message.SenderId,
                SentDate = DateTime.UtcNow,
                Text = $"There are {count} users online"
            });

            return Task.CompletedTask;
        }
        else
        {
            var sessions = _sessionManager.GetAllSessions(x => x.Room == session.Room);
            var count = sessions.Count();

            session.GetMessage(new SystemMessage
            {
                SenderId = message.SenderId,
                SentDate = DateTime.UtcNow,
                Text = $"Users in room {room.Name}: {count}"
            });

            return Task.CompletedTask;
        }
    }
}