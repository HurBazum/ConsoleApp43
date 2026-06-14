using P43.Lib.Messages;
using P43.Lib.Managers.Interfaces;

namespace P43.Lib.Handlers.Server;
public class JoinRoomRequestHandler(IRoomManager roomManager) : IRequestHandler<JoinRoomRequest>
{
    private readonly IRoomManager _roomManager = roomManager;


    public Task HandleAsync(JoinRoomRequest joinRoomRequest, ClientSession session)
    {
        var targetRoom = _roomManager.GetByName(joinRoomRequest.Text);

        if(targetRoom == null)
        {
            session.GetMessage(new SystemMessage()
            {
                SentDate = DateTime.UtcNow,
                SenderId = session.SessionId,
                Text = $"Room '{joinRoomRequest.Text}' does not exist. Please check the name and try again."
            });

            return Task.CompletedTask;
        }
        else
        {
            _roomManager.ChangeRoom(session, targetRoom);

            session.GetMessage(new SystemMessage()
            {
                SentDate = DateTime.UtcNow,
                SenderId = session.SessionId,
                Text = $"You have joined room '{targetRoom.Name}'"
            });

            return Task.CompletedTask;
        }
    }
}