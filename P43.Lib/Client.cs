using System.Buffers;
using P43.Lib.Messages;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace P43.Lib;
public class Client : IDisposable
{
    private readonly Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private readonly MessageWriter _writer = new();
    private readonly Stream_Reader _messageReader = new();
    private bool _authorized = false;
    private bool _isActive = true;
    private readonly StringBuilder _input = new();
    private int _lines = 5;
    private readonly Lock _consoleLock = new();
    private int _messageCount = 0;
    private CancellationTokenSource _cts = new();
    public async Task Start()
    {
        await _socket.ConnectAsync("localhost", 8888);
        _messageReader.OnComplete = HandleMessage;
    }
    
    public async Task SendMessage()
    {
        while(_isActive)
        {
            string input = ProcessTyping();
            if(string.IsNullOrEmpty(input))
            {
                continue;
            }
            
            ClearLine();
            
            _input.Clear();

            MessageBase? message = null;
            
            try
            {
                message = CreateMessage(input);
            }
            catch(Exception ex)
            {
                WrongInput(ex.Message);

                continue;
            }

            if(!_authorized && message is not LoginRequest)
            {
                WrongInput("You must login first using /login <username>");

                continue;
            }
            
            if(message is ExitRequest)
            {
                _isActive = false;
                _socket.Shutdown(SocketShutdown.Both);
                break;
            }
            
            if(message is not null)
            {
                await _writer.WriteAsync(_socket, message);
            }
        }
    }
    
    public async Task ProcessingAsync()
    {
        while(_isActive)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(1024);
            try
            {
                var bytesRead = await _socket.ReceiveAsync(buffer, SocketFlags.None);

                if(bytesRead == 0)
                {
                    _isActive = false;
                    break;
                }

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
        _isActive = false;
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }
    
    private void HandleMessage(MessageBase message)
    {
        lock(_consoleLock)
        {
            _messageCount++;

            if(_lines == Console.BufferHeight)
            {
                _lines = 5;
            }

            if(_messageCount >= Console.BufferHeight)
            {
                ClearLine(line: _lines);
            }

            Console.SetCursorPosition(0, _lines++);

            switch(message)
            {
                case LoginResponse loginResponse:
                    _authorized = loginResponse.Success;
                    Console.WriteLine(loginResponse.Text);
                    break;
                case PublicMessage publicMessage:
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(publicMessage);
                    Console.ResetColor();
                    break;
                case PrivateMessage privateMessage:
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(privateMessage);
                    Console.ResetColor();
                    break;
                case SystemMessage systemMessage:
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine(systemMessage);
                    Console.ResetColor();
                    break;
                default:
                    Console.WriteLine($"Unknown message type, [{message.GetType().Name}]");
                    break;
            }

            int pos = (_input.Length == 0) ? 0 : _input.Length;
            Console.SetCursorPosition(pos, 0);
        }
    }

    private MessageBase? CreateMessage(string input)
    {
        if(!input.StartsWith('/'))
        {
            return new PublicMessage()
            {
                Text = input
            };
        }

        var parts = input[1..].Split(' ');
        var cmd = parts[0].ToLower();

        if(parts.Length == 1)
        {
            return cmd switch
            {
                "who" => new WhoRequest(),
                "exit" => new ExitRequest(),
                "leave" => new LeaveRoomRequest(),
                _ => throw new Exception($"Unknown command: {cmd} without arguments")
            };
        }
        else if(parts.Length == 2)
        {
            var arg = parts[1];

            return cmd switch
            {
                "login" => new LoginRequest() { Text = arg },
                "create" => new CreateRoomRequest() { Text = arg },
                "join" => new JoinRoomRequest() { Text = arg },
                _ => throw new Exception($"Unknown command: {cmd} with argument '{arg}'")
            };
        }
        else if(parts.Length >= 3) 
        {
            var argFisrt = parts[1];
            var argSecond = string.Join(' ', parts[2..]);

            return cmd switch
            {
                "w" => new PrivateMessage() { TargetLogin = argFisrt, Text = argSecond },
                _ => throw new Exception($"Unknown command: '{cmd}' with arguments '{argFisrt}' and '{parts[2]}...'")
            };
        }
        else
        {
            throw new Exception($"Unknown command: {cmd}");
        }
    }

    private string  ProcessTyping()
    {        
        while(true)
        {
            var symbol = Console.ReadKey(intercept: true);
            lock(_consoleLock)
            {
                if(symbol.Key == ConsoleKey.Enter)
                {
                    break;
                }

                if(symbol.Key == ConsoleKey.Backspace && _input.Length > 0)
                {
                    _input.Remove(_input.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else if(symbol.Key != ConsoleKey.Backspace) 
                {
                    _input.Append(symbol.KeyChar);
                    Console.Write(symbol.KeyChar);
                }
            }
        }

        return _input.ToString();
    }

    private void ClearLine(int line = 0)
    {
        string whiteSpaces = new(' ', Console.WindowWidth);

        lock(_consoleLock)
        {
            Console.SetCursorPosition(0, line);
            Console.Write(whiteSpaces);
            Console.SetCursorPosition(0, 0);
        }
    }

    private void WrongInput(string wrongInput)
    {
        Console.WriteLine(wrongInput + " -- Press enter to continue");

        while(true)
        {
            if(Console.ReadKey().Key == ConsoleKey.Enter)
            {
                ClearLine();
                break;
            }
            else
            {
                ClearLine(line: 1);
            }
        }
    }
}