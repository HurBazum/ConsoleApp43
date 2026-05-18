using P43.Lib.Messages;
using System.Buffers;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Channels;

namespace P43.Lib;

public class Server
{
    private Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private ConcurrentDictionary<Guid, ClientSession> _sessions = new();
    private ConcurrentDictionary<string, Room> _rooms = new();
    private static Channel<MessageBase> _requestChannel = Channel.CreateBounded<MessageBase>(1000);
    
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
    
    private async Task BroadcastAsync(MessageBase message, Room? room = null, Guid? senderId = null)
    {
        var session = _sessions[message.SenderId];

        senderId ??= Guid.Empty;

        // echo
        if(room == null)
        {
            session.GetMessage(message);
        }
        else
        {
            foreach(ClientSession clientSession in _sessions.Values.Where(x => x.Room == room && x.SessionId != senderId))
            {
                clientSession.GetMessage(message);
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

                await HandleLoginRequestAsync(loginRequest, systemMessage, session);

                break;
                
            case WhoRequest:

                HandleWhoRequest(session, systemMessage);

                break;

            case ExitRequest:

                HandleExitRequest(session);

                break;
                
            case CreateRoomRequest createRoomRequest:

                HandleCreateRoomRequest(createRoomRequest, session, systemMessage);

                break;

            case JoinRoomRequest joinRoomRequest:

                await HandleJoinRoomRequestAsync(joinRoomRequest, session, systemMessage);

                break;
                
            case LeaveRoomRequest leaveRoomRequest:

                await HandleLeaveRoomRequest(leaveRoomRequest, session, systemMessage);
                
                break;
            case PrivateMessage privateMessage:
                
                HandlePrivateMessage(privateMessage, session, systemMessage);
                    
                break;
        }
    }

    private async Task HandleLoginRequestAsync(LoginRequest loginRequest, SystemMessage? systemMessage, ClientSession session)
    {
        if(_sessions.Values.Any(s => s.Login == loginRequest.Text))
        {
            systemMessage = new()
            {
                SentDate = DateTime.UtcNow,
                SenderId = session.SessionId,
                Text = $"Login {loginRequest.Text} is already taken. Please choose another one."
            };

            session.GetMessage(systemMessage);
        }
        else
        {
            session.SetLogin(loginRequest.Text);

            await HandShakeAsync(loginRequest);
        }
    }

    private void HandleWhoRequest(ClientSession session, SystemMessage? systemMessage)
    {
        var count = _sessions.Values.Count;
        var roomCount = (session.Room is not null) ? _rooms[session.Room!.Name].MemberCount : 0;

        systemMessage = new()
        {
            SentDate = DateTime.UtcNow,
            SenderId = session.SessionId,
            Text = "Online users: " + ((roomCount == 0) ? $"{count}" : $"{roomCount}")
        };

        session.GetMessage(systemMessage);
    }

    private void HandleExitRequest(ClientSession session) => _sessions[session.SessionId].Dispose();

    private void HandleCreateRoomRequest(CreateRoomRequest createRoomRequest, ClientSession session, SystemMessage? systemMessage)
    {
        if(session.Room != null)
        {
            systemMessage = new()
            {
                SentDate = DateTime.UtcNow,
                SenderId = session.SessionId,
                Text = $"You are already in room '{session.Room.Name}'. Please leave it before creating another one."
            };
            session.GetMessage(systemMessage);
            return;
        }

        if(_rooms.TryGetValue(createRoomRequest.Text, out var r))
        {
            systemMessage = new()
            {
                SentDate = DateTime.UtcNow,
                SenderId = session.SessionId,
                Text = $"Room '{createRoomRequest.Text}' already exists. Please choose another name."
            };

            session.GetMessage(systemMessage);
            return;
        }

        if(createRoomRequest.Text.Length > 20)
        {
            systemMessage = new()
            {
                SentDate = DateTime.UtcNow,
                SenderId = session.SessionId,
                Text = $"Room name cannot contains more than 20 characters. Please choose another name."
            };
            session.GetMessage(systemMessage);
        }
        else
        {
            var room = Room.Create(createRoomRequest.Text, session.SessionId);

            _rooms.TryAdd(room.Name, room);

            room.AddMember(session.SessionId);
            session.Room = room;

            systemMessage = new()
            {
                SentDate = DateTime.UtcNow,
                SenderId = session.SessionId,
                Text = $"Room '{room.Name}' was created and you are in that room now"
            };

            session.GetMessage(systemMessage);
        }
    }

    private async Task HandleJoinRoomRequestAsync(JoinRoomRequest joinRoomRequest, ClientSession session, SystemMessage? systemMessage)
    {
        if(session.Room != null)
        {
            systemMessage = new()
            {
                SentDate = DateTime.UtcNow,
                SenderId = session.SessionId,
                Text = $"You are already in room '{session.Room.Name}'. Please leave it before joining another one."
            };
            session.GetMessage(systemMessage);
            return;
        }

        if(!_rooms.TryGetValue(joinRoomRequest.Text, out var targetRoom))
        {
            systemMessage = new()
            {
                SentDate = DateTime.UtcNow,
                SenderId = session.SessionId,
                Text = $"Room '{joinRoomRequest.Text}' does not exist. Please check the name and try again."
            };

            session.GetMessage(systemMessage);
            return;
        }

        session.Room = targetRoom;
        targetRoom.AddMember(session.SessionId);

        systemMessage = new()
        {
            SentDate = DateTime.UtcNow,
            SenderId = session.SessionId,
            Text = $"You have joined room '{targetRoom.Name}'"
        };

        session.GetMessage(systemMessage);

        await BroadcastAsync(new SystemMessage()
        {
            SentDate = DateTime.UtcNow,
            SenderId = session.SessionId,
            Text = $"User '{session.Login}' has joined the room"
        }, targetRoom, session.SessionId);
    }

    private async Task HandleLeaveRoomRequest(LeaveRoomRequest leaveRoomRequest, ClientSession session, SystemMessage? systemMessage)
    {
        if(session.Room == null)
        {
            systemMessage = new()
            {
                SentDate = DateTime.UtcNow,
                SenderId = session.SessionId,
                Text = $"You are not in any room. Please join a room before leaving it."
            };
            session.GetMessage(systemMessage);
            return;
        }

        var room = session.Room;
        session.Room = null;
        
        systemMessage = new()
        {
            SentDate = DateTime.UtcNow,
            SenderId = session.SessionId,
            Text = $"You have left room '{room.Name}'"
        };
        
        session.GetMessage(systemMessage);
        room.RemoveMember(session.SessionId);

        if(room.MemberCount == 0)
        {
            _rooms.TryRemove(room.Name, out _);
        }
        else
        {
            await BroadcastAsync(new SystemMessage()
            {
                SentDate = DateTime.UtcNow,
                SenderId = session.SessionId,
                Text = $"User '{session.Login}' has left the room"
            }, room);
        }
    }

    private void HandlePrivateMessage(PrivateMessage privateMessage, ClientSession session, SystemMessage? systemMessage)
    {
        var targetSession = _sessions.Values.FirstOrDefault(s => s.Login == privateMessage.TargetLogin);

        if(targetSession == null)
        {
            systemMessage = new()
            {
                SentDate = DateTime.UtcNow,
                SenderId = session.SessionId,
                Text = $"User '{privateMessage.TargetLogin}' does not exist"
            };

            session.GetMessage(systemMessage);
            return;
        }


        session.GetMessage(privateMessage);
        if(session.SessionId != targetSession.SessionId)
        {
            targetSession.GetMessage(privateMessage);
        }
    }
}