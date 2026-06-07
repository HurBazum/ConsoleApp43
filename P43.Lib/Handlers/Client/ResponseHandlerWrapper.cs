using P43.Lib.Messages;
using P43.Lib.Managers.Interfaces;

namespace P43.Lib.Handlers.Client;
public class ResponseHandlerWrapper<TMessage>(IResponseHandler<TMessage> handler) : IResponseHandlerNG where TMessage : IMessageBase
{
    private readonly IResponseHandler<TMessage> _handler = handler;

    public Task HandleAsync(IMessageBase message) => _handler.HandleAsync((TMessage)message);
}