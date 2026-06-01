using P43.Lib.Managers.Interfaces;
using P43.Lib.Messages;

namespace P43.Lib.Handlers.Server;
public class RequestHandlersDispatcher
{
    private readonly Dictionary<Type, object> _handlers = new();

    public void SetHandlers(params object[] handlers)
    {
        foreach(var handler in handlers)
        {
            var type = handler
                .GetType()
                .GetInterfaces()
                .First(
                    i => i.IsGenericType == true &&
                    i.GetGenericTypeDefinition() == typeof(IMessageHandler<>))
                .GetGenericArguments()[0];

            _handlers[type] = handler;
        }
    }

    public async Task DispatchAsync(IMessageBase message, ClientSession session)
    {
        _handlers.TryGetValue(message.GetType(), out var handler);

        if(handler is not null)
        {
            await ((dynamic)handler).HandleAsync((dynamic)message, session);
        }
    }
}