using P43.Lib;

namespace P43.ClientApp;

class Program
{
    private static async Task Main(string[] args)
    {
        Client client = new();
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