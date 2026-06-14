using Microsoft.Extensions.DependencyInjection;
using P43.Wpf.ViewModels;

namespace P43.Wpf.Views.Windows;
public static class WindowRegister
{
    public static IServiceCollection AddWindows(this IServiceCollection services) =>
        services.AddSingleton<MainWindow>();
}