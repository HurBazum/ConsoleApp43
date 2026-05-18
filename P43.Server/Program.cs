using P43.Lib;

namespace P43.ServerApp;

class Pragram
{
    private static async Task Main(string[] args)
    {
        Server server = new();

        await server.Start();
    }
}