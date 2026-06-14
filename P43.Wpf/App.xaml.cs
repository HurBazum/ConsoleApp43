using P43.Wpf.ViewModels;
using P43.Wpf.Infrastructure.Stores;
using P43.Wpf.Views.Windows;
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
        public IHost _host = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _host = Host.CreateDefaultBuilder(e.Args)
                .ConfigureServices(ConfigureServices)
                .Build();

            _host.Start();

            MainWindow main = _host.Services.GetRequiredService<MainWindow>();

            INavigation nav = _host.Services.GetRequiredService<INavigation>();

            nav.Next<StartViewModel>();

            main.Show();
        }

        

        public static void ConfigureServices(HostBuilderContext host, IServiceCollection services) => services
            .AddStores()
            .AddViewModels()
            .AddWindows();
    }
}