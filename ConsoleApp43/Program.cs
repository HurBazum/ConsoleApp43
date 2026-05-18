using System.Text;

namespace ConsoleApp43;
internal class Program
{
    static async Task Main(string[] args)
    {
        Server server = new();
        _ = server.Start();
        
        Client client = new();
        await client.Start();
        
        await Task.WhenAll(client.ProcessingAsync(), client.SendMessage());
    }
    
    private static void PrintBytes(byte[] bytes)
    {
        Console.WriteLine($"Data: {Encoding.UTF8.GetString(bytes)}");
    }
}