using P43.Lib.Messages;

namespace P43.Lib.Managers.Interfaces;

public interface IResponseHandler<TMessage> where TMessage : IMessageBase
{
    Task HandleAsync(TMessage message);
}