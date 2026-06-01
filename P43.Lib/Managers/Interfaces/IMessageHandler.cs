using P43.Lib.Messages;

namespace P43.Lib.Managers.Interfaces;
public interface IMessageHandler<TMessage> where TMessage : IMessageBase
{
    public Task HandleAsync(TMessage message, ClientSession session);
}