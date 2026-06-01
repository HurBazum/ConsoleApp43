using P43.Lib;
using P43.Lib.Managers;
using P43.Lib.Handlers.Server;

namespace P43.ServerApp;

class Pragram
{
    private static async Task Main(string[] args)
    {
        var sessionManager = new SessionManager();
        var roomManager = new RoomManager();

        var dispatcher = new RequestHandlersDispatcher();

        object[] handlers = [
            new CreateRoomRequestHandler(roomManager),
            new JoinRoomRequestHandler(roomManager, sessionManager),
            new ExitRequestHandler(),
            new LeaveRoomRequestHandler(sessionManager, roomManager),
            new PrivateMessageHandler(sessionManager),
            new LoginRequestHandler(sessionManager),
            new WhoRequestHandler(sessionManager),
            new PublicMessageHandler(sessionManager)
        ];

        dispatcher.SetHandlers(handlers);

        Server server = new(sessionManager, roomManager, dispatcher);

        await server.Start();
    }
}