using P43.Lib.Managers.Interfaces;
using P43.Lib.Messages;

namespace P43.Lib.Handlers.Server;
public class RequestHandlersDispatcher
{
    private readonly Dictionary<Type, IRequestHandlerNG> _handlers = new();

    public void SetHandlers(params object[] handlers)
    {
        foreach(object handler in handlers)
        {
            Type handlerType = handler.GetType();

            Type? messageType = handlerType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHandler<>))
                .GetGenericArguments()[0];

            if(messageType is null)
            {
                continue;
            }

            Type wrapperType = typeof(RequestHandlerWrapper<>);

            Type wrapper = wrapperType.MakeGenericType(messageType);

            if(Activator.CreateInstance(wrapper, handler) is IRequestHandlerNG wrapperInstance)
            {
                _handlers[messageType] = wrapperInstance;
            }
        }
    }

    public async Task DispatchAsync(IMessageBase message, ClientSession session)
    {
        _handlers.TryGetValue(message.GetType(), out var handler);

        if(handler is not null)
        {
            await handler.HandleAsync(message, session);
        }
    }
}