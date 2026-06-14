using P43.Lib;
using P43.Lib.Handlers.Client;

namespace P43.ClientApp;

class Program
{
    private static async Task Main(string[] args)
    {
        ClientState state = new();
        object[] handlers = 
            [ 
            new LoginResponseHandler(state), 
            new PrivateMessageHandler(), 
            new PublicMessageHandler(), 
            new SystemMessageHandler()
            ];

        var dispatcher = new ResponseHandlersDispatcher();

        dispatcher.SetHandlers(handlers);

        ClientService client = new(dispatcher, state);

        await client.Start();

        Task getting = client.ProcessingAsync();
        Task sending = client.SendMessage();

        Task completedTask = await Task.WhenAny(getting, sending);

        if(completedTask == getting)
        {
            Console.WriteLine("Соединение разорвано сервером.");            
        }
        else
        {
            Console.WriteLine("Вы завершили соединение.");
        }

        Console.ReadLine();
    }
}