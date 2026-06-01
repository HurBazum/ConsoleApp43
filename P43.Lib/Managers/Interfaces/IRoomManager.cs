namespace P43.Lib.Managers.Interfaces;
public interface IRoomManager
{
    bool AddRoom(Room room);
    Room? GetByName(string name);
    Room? RemoveRoom(string name);
}