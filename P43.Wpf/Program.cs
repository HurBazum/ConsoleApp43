using Microsoft.Extensions.Hosting;

namespace P43.Wpf;
public class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        App app = new();
        app.InitializeComponent();
        app.Run();
    }
    public static IHostBuilder CreateDefaultHostBuilder(string[] args) => Host.CreateDefaultBuilder(args).ConfigureServices(App.ConfigureServices);
}