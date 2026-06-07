using P43.Lib.Messages;

namespace P43.Lib.Managers.Interfaces;
public interface IRequestHandlerNG
{
    Task HandleAsync(IMessageBase message, ClientSession session);
}