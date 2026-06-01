using P43.Lib.Messages;
using P43.Lib.Managers.Interfaces;

namespace P43.Lib.Handlers.Client;

public class LoginResponseHandler(ClientState state) : IResponseHandler<LoginResponse>
{
    private readonly ClientState _state = state;

    public Task HandleAsync(LoginResponse message)
    {
        if(message.Success)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            _state.Authorize();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
        }

        Console.WriteLine(message.Text);
        Console.ResetColor();

        return Task.CompletedTask;
    }
}