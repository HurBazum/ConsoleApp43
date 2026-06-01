using P43.Lib.Messages;
using P43.Lib.Managers.Interfaces;

namespace P43.Lib.Handlers.Server;
public class PublicMessageHandler(ISessionManager sessionManager) : IMessageHandler<PublicMessage>
{
    private readonly ISessionManager _sessionManager = sessionManager;

    public Task HandleAsync(PublicMessage message, ClientSession session)
    {
        if(session.Room == null)
        {
            session.GetMessage(message);
        }
        else
        {
            var sessions = _sessionManager.GetAllSessions(x => x.Room == session.Room && x.SessionId != session.SessionId);

            foreach(var s in sessions)
            {
                s.GetMessage(message);
            }

        }

        return Task.CompletedTask;
    }
}