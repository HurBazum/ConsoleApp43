using System.Buffers;
using System.Net.Sockets;
using System.Threading.Channels;
using P43.Lib.Messages;
using System.Text.Json;

namespace P43.Lib;

public class ClientSession : IDisposable
{
    public Guid SessionId { get; init; } = Guid.NewGuid();
    
    private readonly Socket _socket;
    private readonly Stream_Reader _parser;
    private readonly MessageWriter _writer;
    private readonly Channel<IMessageBase> _messageChannel = Channel.CreateBounded<IMessageBase>(100);

    public event EventHandler<ReceivedMessageEventArgs>? OnMessageReceived;    
    public event Action<Guid>? OnDisconnected;
    
    public Room? Room { get; set; }

    public string Login { get; private set; } = string.Empty;
    
    public ClientSession(Socket socket)
    {
        _socket = socket;
        _parser = new Stream_Reader();
        _writer = new MessageWriter();
        
        _parser.OnComplete = HandleMessage;
    }
    
    public async Task ProcessingAsync()
    {
        try
        {
            while(true)
            {
                var buffer = ArrayPool<byte>.Shared.Rent(1024);
                try
                {
                    var bytesRead = await _socket.ReceiveAsync(buffer, SocketFlags.None);
                    if(bytesRead == 0)
                    {
                        break;
                    }
                    
                    _parser.Parse(buffer.AsSpan(0, bytesRead));
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }
        finally
        {
            OnDisconnected?.Invoke(SessionId);
            Dispose();
        }
    }
    
    private Task HandleMessage(RentedBufferOwner message)
    {
        try
        {
            var msg = JsonSerializer.Deserialize<IMessageBase>(message.Memory.Span);
            msg.SenderId = SessionId;
            msg.SentDate = DateTime.UtcNow;
            msg.Login = Login;

            OnMessageReceived?
                .Invoke(this,
                new ReceivedMessageEventArgs
                {
                    Context = new()
                    {
                        Message = msg,
                        Session = this
                    }
                });
        }
        finally
        {
            message.Dispose();
        }

        return Task.CompletedTask;
    }
    
    public async Task HandleChannel()
    {
        await foreach(IMessageBase message in _messageChannel.Reader.ReadAllAsync())
        {
            await SendAsync(message);
        }
    }
    
    public async Task SendAsync(IMessageBase messageBase) => await _writer.WriteAsync(_socket, messageBase);
    
    public void GetMessage(IMessageBase message) => _messageChannel.Writer.TryWrite(message);
    
    public void Dispose()
    {
        _messageChannel.Writer.Complete();
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }
    
    public void SetLogin(string login)
    {
        Login = login;
    }    
}