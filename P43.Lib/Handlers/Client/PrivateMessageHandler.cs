using P43.Lib.Messages;
using P43.Lib.Managers.Interfaces;

namespace P43.Lib.Handlers.Client;
public class PrivateMessageHandler : IResponseHandler<PrivateMessage>
{
    public Task HandleAsync(PrivateMessage message)
    {
        Console.ForegroundColor = ConsoleColor.DarkMagenta;
        Console.WriteLine(message);
        Console.ResetColor();
        
        return Task.CompletedTask;
    }
}