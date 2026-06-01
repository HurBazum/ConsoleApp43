using P43.Lib.Managers.Interfaces;
using P43.Lib.Messages;

namespace P43.Lib.Handlers.Client;
public class PublicMessageHandler : IResponseHandler<PublicMessage>
{
    public Task HandleAsync(PublicMessage message)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(message);
        Console.ResetColor();
        
        return Task.CompletedTask;
    }
}