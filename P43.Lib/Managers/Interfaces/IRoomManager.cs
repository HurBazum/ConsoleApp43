namespace P43.Lib.Managers.Interfaces;
public interface IRoomManager
{
    event Action<ClientSession, Room>? OnJoiningRoom;
    event Action<ClientSession, Room>? OnLeavingRoom;
    bool AddRoom(Room room);
    Room? GetByName(string name);
    Room? RemoveRoom(string name);
    void ChangeRoom(ClientSession session, Room? newRoomName);
}