using P43.Lib.Managers.Interfaces;
using System.Collections.Concurrent;

namespace P43.Lib.Managers;
public class RoomManager : IRoomManager
{
    private ConcurrentDictionary<string, Room> _rooms = new();

    public event Action<ClientSession, Room>? OnLeavingRoom;
    public event Action<ClientSession, Room>? OnJoiningRoom;

    public bool AddRoom(Room room) => _rooms.TryAdd(room.Name, room);
    public Room? GetByName(string name) => _rooms.TryGetValue(name, out var room) ? room : null;
    public Room? RemoveRoom(string name)
    {
        _rooms.TryRemove(name, out var room);
        return room;        
    }
    public void ChangeRoom(ClientSession session, Room? newRoom)
    {
        if(session.Room != null)
        {
            Room? oldRoom = GetByName(session.Room.Name);
            oldRoom?.RemoveMember(session.SessionId);
            session.Room = null;
            OnLeavingRoom?.Invoke(session, oldRoom!);
        }
        
        
        if(newRoom != null)
        {
            newRoom.AddMember(session.SessionId);
            session.Room = newRoom;
            OnJoiningRoom?.Invoke(session, newRoom);
        }
        else
        {
            return;
        }
    }
}