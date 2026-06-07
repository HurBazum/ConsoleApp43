using P43.Lib.Managers.Interfaces;
using P43.Lib.Messages;

namespace P43.Lib.Handlers.Server;
public class LoginRequestHandler(ISessionManager sessionManager) : IRequestHandler<LoginRequest>
{
    private readonly ISessionManager _sessionManager = sessionManager;

    public Task HandleAsync(LoginRequest message, ClientSession session)
    {
        if(_sessionManager.GetBy(x => x.Login == message.Text) != null)
        {
            session.GetMessage(new LoginResponse
            {
                SenderId = message.SenderId,
                SentDate = DateTime.UtcNow,
                Success = false,
                Text = $"Login {message.Text} already taken"
            });
        }
        else
        {
            session.SetLogin(message.Text);
            session.GetMessage(new LoginResponse
            {
                SenderId = message.SenderId,
                SentDate = DateTime.UtcNow,
                Success = true,
                Text = $"Login {message.Text} successful"
            });
        }

        return Task.CompletedTask;
    }
}