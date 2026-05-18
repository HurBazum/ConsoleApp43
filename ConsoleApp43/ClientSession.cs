using System.Buffers;
using System.Net.Sockets;
using System.Threading.Channels;
using ConsoleApp43.Messages;
using System.Text.Json;

namespace ConsoleApp43;
public class ClientSession : IDisposable
{
    public Guid SessionId { get; init; } = Guid.NewGuid();
    
    private readonly Socket _socket;
    private readonly Stream_Reader _parser;
    private readonly MessageWriter _writer;
    private readonly Channel<MessageBase> _messageChannel = Channel.CreateBounded<MessageBase>(100);

    public event EventHandler<ReceivedMessageEventArgs>? OnMessageReceived;    
    public event Action<Guid>? OnDisconnected;
    
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
    
    private async Task HandleMessage(byte[] message, int length)
    {
        var msg = JsonSerializer.Deserialize<MessageBase>(message.AsSpan(0, length));
        msg.SenderId = SessionId;
        msg.SentDate = DateTime.UtcNow;
        msg.Login = Login;
        
        OnMessageReceived?.Invoke(this, new ReceivedMessageEventArgs { Message = msg });
        
        ArrayPool<byte>.Shared.Return(message);
    }
    
    public async Task HandleChannel()
    {
        await foreach(MessageBase message in _messageChannel.Reader.ReadAllAsync())
        {
            await SendAsync(message);
        }
    }
    
    public async Task SendAsync(MessageBase messageBase) => await _writer.WriteAsync(_socket, messageBase);
    
    public void GetMessage(MessageBase message) => _messageChannel.Writer.TryWrite(message);
    
    public void Dispose()
    {
        _messageChannel.Writer.Complete();
        _socket.Close();
    }
    
    public void SetLogin(string login)
    {
        Login = login;
    }    
}