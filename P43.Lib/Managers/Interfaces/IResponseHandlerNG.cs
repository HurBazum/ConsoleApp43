using P43.Lib.Messages;

namespace P43.Lib.Managers.Interfaces;
public interface IResponseHandlerNG
{
    Task HandleAsync(IMessageBase message);
}