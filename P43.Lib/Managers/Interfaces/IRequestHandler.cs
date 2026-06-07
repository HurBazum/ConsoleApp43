using P43.Lib.Messages;

namespace P43.Lib.Managers.Interfaces;
public interface IRequestHandler<TMessage> where TMessage : IMessageBase
{
    Task HandleAsync(TMessage message, ClientSession session);
}