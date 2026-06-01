namespace P43.Lib.Managers.Interfaces;
public interface ISessionManager
{
    bool AddSession(ClientSession session);
    ClientSession? GetByKey(Guid key);
    ClientSession? RemoveSession(Guid sessionId);
    IQueryable<ClientSession> GetAllSessions(Func<ClientSession, bool>? predicate = null);
    ClientSession? GetBy(Func<ClientSession, bool> predicate);
    int Count();
}