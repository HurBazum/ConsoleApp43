using System.Buffers;
using ConsoleApp43.Messages;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ConsoleApp43;
public class Client : IDisposable
{
    private Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private readonly MessageWriter _writer = new();
    private readonly Stream_Reader _messageReader = new();
    public async Task Start()
    {
        await _socket.ConnectAsync("localhost", 8888);
        _messageReader.OnComplete = HandleMessage;
    }
    
    public async Task SendMessage()
    {
        while(true)
        {
            string str = Console.ReadLine();
            if(string.IsNullOrEmpty(str))
            {
                return;
            }
            
            MessageBase? message = null;
            if(str.StartsWith('/'))
            {
                if(str.Contains(' '))
                {
                    var cmdArray = str[1..].Split(' ');
                    
                    if(cmdArray.Length == 2 && cmdArray[0] == "login")
                    {
                        message = new LoginRequest()
                        {
                            Text = cmdArray[1]
                        };
                    }
                    else
                    {
                        Console.WriteLine("Unknown command");
                    }
                }
                else
                {
                    switch(str[1..])
                    {
                        case "who":
                            message = new WhoRequest();
                            break;
                        case "exit":break;
                        default:
                            Console.WriteLine("Unknown command");
                            break;
                    }
                }
            }
            else
            {
                message = new PublicMessage()
                {
                    Text = str
                };
            }

            if(message is not null)
            {
                await _writer.WriteAsync(_socket, message);
            }
        }
    }
    
    public async Task ProcessingAsync()
    {
        while(true)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(1024);
            try
            {
                await Task.Delay(100);
                var bytesRead = await _socket.ReceiveAsync(buffer, SocketFlags.None);
                _messageReader.Parse(buffer.AsSpan(0, bytesRead));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }
    }
    
    private async Task HandleMessage(byte[] message, int length)
    {
        var str = Encoding.UTF8.GetString(message[0..length]);

        MessageBase deserialized = JsonSerializer.Deserialize<MessageBase>(str);
        
        HandleMessage(deserialized);
        
        ArrayPool<byte>.Shared.Return(message);
    }
    
    public void Dispose()
    {
        _socket.Dispose();
    }
    
    private void HandleMessage(MessageBase message)
    {
        switch(message)
        {
            case LoginResponse loginResponse:
                Console.WriteLine(loginResponse.Text);
                break;
            case PublicMessage publicMessage:
                Console.WriteLine(publicMessage);
                break;
            case PrivateMessage privateMessage:
                Console.WriteLine(privateMessage);
                Console.SetCursorPosition
                break;
            case SystemMessage systemMessage:
                Console.WriteLine(systemMessage);
                break;
            default:
                Console.WriteLine($"Unknown message type, [{message.GetType().Name}]");
                break;
        }
    }
}