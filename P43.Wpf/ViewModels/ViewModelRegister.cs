using Microsoft.Extensions.DependencyInjection;

namespace P43.Wpf.ViewModels;
public static class ViewModelRegister
{
    public static void AddViewModels(this IServiceCollection services) => services
        .AddSingleton<MainViewModel>()
        .AddTransient<StartViewModel>()
        .AddTransient<LoginViewModel>()
        .AddTransient<RegisterViewModel>();        
}