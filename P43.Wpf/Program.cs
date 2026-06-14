using Microsoft.Extensions.Hosting;
using System.Windows;

namespace P43.Wpf;
public class Program
{
    [STAThread]
    public static async Task Main(string[] args)
    {
        App app = new();
        app.InitializeComponent();
        app.Run();
    }
    public static IHostBuilder CreateDefaultHostBuilder(string[] args) => Host.CreateDefaultBuilder(args).ConfigureServices(App.ConfigureServices);
}