using P43.Lib.Managers.Interfaces;
using P43.Lib.Messages;

namespace P43.Lib.Handlers.Client;
public class ResponseHandlersDispatcher
{
    private readonly Dictionary<Type, IResponseHandlerNG> _handlers = new();

    public void SetHandlers(params object[] handlers)
    {
        foreach(var handler in handlers)
        {
            Type handlerType = handler.GetType();
            Type? messageType = handlerType.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IResponseHandler<>)).GetGenericArguments()[0];

            if(messageType is null)
            {
                continue;
            }

            Type wrapperType = typeof(ResponseHandlerWrapper<>);

            Type wrapper = wrapperType.MakeGenericType(messageType);

            if(Activator.CreateInstance(wrapper, handler) is IResponseHandlerNG wrapperInstance)
            {
                _handlers[messageType] = wrapperInstance;
            }
        }
    }

    public async Task DispatchAsync(IMessageBase message)
    {
        _handlers.TryGetValue(message.GetType(), out var handler);

        if(handler is not null)
        {
            await handler.HandleAsync(message);
        }
    }
}