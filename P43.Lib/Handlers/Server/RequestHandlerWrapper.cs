using P43.Lib.Messages;
using P43.Lib.Managers.Interfaces;

namespace P43.Lib.Handlers.Server;
public class RequestHandlerWrapper<TMessage>(IRequestHandler<TMessage> handler) : IRequestHandlerNG where TMessage : IMessageBase
{
    private readonly IRequestHandler<TMessage> _handler = handler;

    public Task HandleAsync(IMessageBase message, ClientSession session) => _handler.HandleAsync((TMessage)message, session);
}