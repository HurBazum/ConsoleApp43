using P43.Lib.Managers.Interfaces;
using P43.Lib.Messages;

namespace P43.Lib.Handlers.Server;
public class PrivateMessageHandler(ISessionManager sessionManager) : IRequestHandler<PrivateMessage>
{
    private readonly ISessionManager _sessionManager = sessionManager;

    public Task HandleAsync(PrivateMessage message, ClientSession session)
    {
        var targetSession = _sessionManager.GetBy(s => s.Login == message.TargetLogin);

        if(targetSession == null)
        {
            session.GetMessage(new SystemMessage()
            {
                SentDate = DateTime.UtcNow,
                SenderId = session.SessionId,
                Text = $"User '{message.TargetLogin}' does not exist"
            });

            return Task.CompletedTask;
        }

        session.GetMessage(message);

        if(session.SessionId != targetSession.SessionId)
        {
            targetSession.GetMessage(message);
        }

        return Task.CompletedTask;
    }
}