using P43.Lib.Messages;
using P43.Lib.Managers.Interfaces;

namespace P43.Lib.Handlers.Client;
public class SystemMessageHandler : IResponseHandler<SystemMessage>
{
    public Task HandleAsync(SystemMessage message)
    {
        Console.ForegroundColor = ConsoleColor.DarkBlue;
        Console.WriteLine(message);
        Console.ResetColor();
        
        return Task.CompletedTask;
    }
}