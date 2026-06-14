using P43.Lib;
using P43.Lib.Managers;
using P43.Lib.Managers.Interfaces;
using P43.Lib.Handlers.Server;

namespace P43.ServerApp;

class Program
{
    private static async Task Main(string[] args)
    {
        var sessionManager = new SessionManager();
        IRoomManager roomManager = new RoomManager();

        RoomNotificationService notifier = new(sessionManager);

        roomManager.OnJoiningRoom += notifier.Notify;
        roomManager.OnLeavingRoom += notifier.Notify;

        var dispatcher = new RequestHandlersDispatcher();

        object[] handlers = [
            new CreateRoomRequestHandler(roomManager),
            new JoinRoomRequestHandler(roomManager),
            new ExitRequestHandler(),
            new LeaveRoomRequestHandler(roomManager),
            new PrivateMessageHandler(sessionManager),
            new LoginRequestHandler(sessionManager),
            new WhoRequestHandler(sessionManager),
            new PublicMessageHandler(sessionManager)
        ];

        dispatcher.SetHandlers(handlers);

        Server server = new(sessionManager, dispatcher);

        await server.Start();
    }
}