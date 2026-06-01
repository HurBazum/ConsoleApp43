using P43.Lib.Messages;
using P43.Lib.Managers.Interfaces;

namespace P43.Lib.Handlers.Server;
public class CreateRoomRequestHandler(IRoomManager roomManager) : IMessageHandler<CreateRoomRequest>
{
    private readonly IRoomManager _roomManager = roomManager;

    public Task HandleAsync(CreateRoomRequest message, ClientSession session)
    {
        if(session.Room != null)
        {
            session.GetMessage(new SystemMessage
            {
                SenderId = session.SessionId,
                SentDate = DateTime.UtcNow,
                Text = "You are already in a room. Please leave the current room before creating a new one."
            });

            return Task.CompletedTask;
        }

        if(_roomManager.GetByName(message.Text) != null)
        {
            session.GetMessage(new SystemMessage
            {
                SenderId = session.SessionId,
                SentDate = DateTime.UtcNow,
                Text = $"A room with the name '{message.Text}' already exists."
            });

            return Task.CompletedTask;
        }

        var room = Room.Create(message.Text, message.SenderId);
        _roomManager.AddRoom(room);

        session.Room = room;

        session.GetMessage(new SystemMessage
        {
            SenderId = session.SessionId,
            SentDate = DateTime.UtcNow,
            Text = $"Room '{message.Text}' created successfully. You are in now"
        });

        return Task.CompletedTask;
    }
}