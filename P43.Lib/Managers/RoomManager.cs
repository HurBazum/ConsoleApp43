using P43.Lib.Managers.Interfaces;
using System.Collections.Concurrent;

namespace P43.Lib.Managers;
public class RoomManager : IRoomManager
{
    private ConcurrentDictionary<string, Room> _rooms = new();

    public bool AddRoom(Room room) => _rooms.TryAdd(room.Name, room);

    public Room? GetByName(string name) => _rooms.TryGetValue(name, out var room) ? room : null;

    public Room? RemoveRoom(string name)
    {
        _rooms.TryRemove(name, out var room);
        return room;        
    }
}