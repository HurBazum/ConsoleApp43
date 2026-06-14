using P43.Lib.Managers.Interfaces;
using P43.Lib.Messages;

namespace P43.Lib.Managers;
public class RoomNotificationService(ISessionManager sessionManager)
{
    ISessionManager _sessionManager = sessionManager;


    public void Notify(ClientSession smb, Room room)
    {
        IQueryable<ClientSession> sessions = _sessionManager.GetAllSessions(x => x.Room == room && x.SessionId != smb.SessionId);
        IMessageBase message;

        if(smb.Room == room)
        {
            message = new SystemMessage()
            {
                SentDate = DateTime.UtcNow,
                SenderId = smb.SessionId,
                Text = $"User '{smb.Login}' has joined the room"
            };
        }
        else
        {
            message = new SystemMessage()
            {
                SentDate = DateTime.UtcNow,
                SenderId = smb.SessionId,
                Text = $"User '{smb.Login}' has left the room"
            };
        }

        foreach(ClientSession session in sessions)
        {
            session.GetMessage(message);
        }
    }
}