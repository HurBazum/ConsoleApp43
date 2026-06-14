using P43.Wpf.ViewModels;
using System.Windows;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace P43.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;
        public IHost Host => _host ??= Program.CreateDefaultHostBuilder(Environment.GetCommandLineArgs()).Build();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var host = Host;

            host.StartAsync().ConfigureAwait(false);

            var main = new MainWindow();

            var vm = host.Services.GetRequiredService<MainViewModel>();

            main.DataContext = vm;

            main.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            var host = Host;

            host.StopAsync().ConfigureAwait(false);

            host.Dispose();
        }

        public static void ConfigureServices(HostBuilderContext host, IServiceCollection services) => services.AddViewModels();
    }
}