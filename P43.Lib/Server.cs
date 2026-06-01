using P43.Lib.Managers.Interfaces;
using P43.Lib.Managers;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;
using P43.Lib.Handlers.Server;

namespace P43.Lib;

public class Server(SessionManager sessionManager, RoomManager roomManager, RequestHandlersDispatcher dispatcher)
{
    private Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private static Channel<MessageContext> _requestChannel = Channel.CreateBounded<MessageContext>(1000);

    private readonly ISessionManager _sessionManager = sessionManager;
    private readonly IRoomManager _roomManager = roomManager;

    private readonly RequestHandlersDispatcher _dispatcher = dispatcher;

    public async Task Start()
    {
        _socket.Bind(new IPEndPoint(IPAddress.Any, 8888));
        _socket.Listen(100);
        _ = ResponseAsync();

        Console.WriteLine("Сервер запущен. Ожидаем клиентов. . .");

        while(true)
        {
            var client = await _socket.AcceptAsync();
            
            ClientSession session = new(client);
            if(_sessionManager.AddSession(session))
            {
                session.OnMessageReceived += PrepareResponseAsync;
                session.OnDisconnected += (id) =>
                {
                    var removedSession = _sessionManager.RemoveSession(id);
                    Console.WriteLine($"Session {removedSession.SessionId} disconnected.");
                };
                _ = HandleClient(session);
            }
        }
    }
    
    private async Task HandleClient(ClientSession cs)
    {
        Task processing = cs.ProcessingAsync();
        Task responsing = cs.HandleChannel();
        
        await Task.WhenAll(processing, responsing);
    }
    
    private void Log(byte[] message, int length, string client)
    {
        var str = Encoding.UTF8.GetString(message[0..length]);
        Console.WriteLine($"{client}: {str}");
        ArrayPool<byte>.Shared.Return(message);
    }
    
    private void PrepareResponseAsync(object? sender, ReceivedMessageEventArgs e)
    {
        _requestChannel.Writer.TryWrite(e.Context);
    }
    
    private async Task ResponseAsync()
    {
        await foreach(MessageContext ctx in _requestChannel.Reader.ReadAllAsync())
        { 
            await _dispatcher.DispatchAsync(ctx.Message, ctx.Session);
        }
    }    
}