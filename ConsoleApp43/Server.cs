using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Buffers;
using System.Collections.Concurrent;
using System.Threading.Channels;
using ConsoleApp43.Messages;

namespace ConsoleApp43;
internal class Server
{
    private Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private ConcurrentDictionary<Guid, ClientSession> _sessions = new();
    private static Channel<MessageBase> _requestChannel = Channel.CreateBounded<MessageBase>(1000);
    
    public async Task Start()
    {
        _socket.Bind(new IPEndPoint(IPAddress.Any, 8888));
        _socket.Listen(100);
        _ = ResponseAsync();
        while(true)
        {
            var client = await _socket.AcceptAsync();
            
            ClientSession session = new(client);
            _sessions.TryAdd(session.SessionId, session);
            session.OnMessageReceived += PrepareResponseAsync;
            session.OnDisconnected += (id) =>
            {
                _sessions.TryRemove(id, out _);
                Console.WriteLine($"Session {id} disconnected.");
            };
            _ = HandleClient(session);
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
        _requestChannel.Writer.TryWrite(e.Message);
    }
    
    private async Task ResponseAsync()
    {
        await foreach(MessageBase message in _requestChannel.Reader.ReadAllAsync())
        { 
            if(message is PublicMessage pm)
                await BroadcastAsync(pm);
            else
                await CmdResponseAsync(message);                
        }
    }
    
    private async Task BroadcastAsync(MessageBase message)
    {            
        foreach(ClientSession session in _sessions.Values)
        {
            if(session.SessionId == message.SenderId)
            {
                session.GetMessage(message);
            }
        }
    }
    
    private async Task HandShakeAsync(MessageBase message)
    {
        var session = _sessions[message.SenderId];
        
        LoginResponse loginResponse = new()
        {
            SentDate = DateTime.UtcNow,
            Success = true,
            SenderId = session.SessionId,
            Text = $"Welcome, {session.Login}!",
            Login = session.Login
        };
        
        session.GetMessage(loginResponse);
    }
    
    private async Task CmdResponseAsync(MessageBase message)
    {
        ClientSession session = _sessions[message.SenderId];
        SystemMessage systemMessage = null;
        
        switch(message)
        {
            case LoginRequest loginRequest:
                if(_sessions.Values.Any(s => s.Login == message.Text))
                {
                    systemMessage = new()
                    {
                        SentDate = DateTime.UtcNow,
                        SenderId = session.SessionId,
                        Text = $"Login {message.Text} is already taken. Please choose another one."
                    };
                    
                    session.GetMessage(systemMessage);
                }
                else
                {
                    session.SetLogin(message.Text);
                    await HandShakeAsync(message);
                };           
                break;
                
            case WhoRequest whoRequest:
                var logins = _sessions.Values.Select(s => s.Login).ToArray();
                systemMessage = new()
                {
                    SentDate = DateTime.UtcNow,
                    SenderId = session.SessionId,
                    Text = $"Online users: {string.Join(", ", logins)}"
                };
                
                session.GetMessage(systemMessage);
                break;
        }
    }
}