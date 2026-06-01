using P43.Lib.Managers.Interfaces;
using System.Collections.Concurrent;

namespace P43.Lib.Managers;
public class SessionManager : ISessionManager
{
    private ConcurrentDictionary<Guid, ClientSession> _sessions = new();

    public bool AddSession(ClientSession session) => _sessions.TryAdd(session.SessionId, session);
    public ClientSession? GetByKey(Guid key) => _sessions.TryGetValue(key, out var session) ? session : null;
    public ClientSession? RemoveSession(Guid sessionId)
    {
        _sessions.TryRemove(sessionId, out var session);
        return session;
    }

    public IQueryable<ClientSession> GetAllSessions(Func<ClientSession, bool>? predicate = null)
    {
        var sessions = _sessions.Values.AsQueryable();
        if (predicate != null)
        {
            sessions = sessions.Where(predicate).AsQueryable();
        }
        return sessions;
    }

    public ClientSession? GetBy(Func<ClientSession, bool> predicate) => _sessions.Values.FirstOrDefault(predicate);
    public int Count() => _sessions.Count;
}