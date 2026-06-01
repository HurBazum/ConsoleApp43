using P43.Lib.Messages;
using P43.Lib.Managers.Interfaces;

namespace P43.Lib.Handlers.Server;
public class ExitRequestHandler : IMessageHandler<ExitRequest>
{
    public Task HandleAsync(ExitRequest message, ClientSession session)
    {
        session.Dispose();

        return Task.CompletedTask;
    }
}