using P43.Lib.Managers.Interfaces;
using P43.Lib.Messages;

namespace P43.Lib.Handlers.Server;
public class LeaveRoomRequestHandler(IRoomManager roomManager) : IRequestHandler<LeaveRoomRequest>
{
    private readonly IRoomManager _roomManager = roomManager;

    public Task HandleAsync(LeaveRoomRequest message, ClientSession session)
    {
        if(session.Room == null)
        {
            session.GetMessage(new SystemMessage()
            {
                SentDate = DateTime.UtcNow,
                SenderId = session.SessionId,
                Text = $"You are not in any room. Please join a room before leaving it."
            });

            return Task.CompletedTask;
        }
        else
        {
            _roomManager.ChangeRoom(session, null);

            return Task.CompletedTask;
        }
    }
}