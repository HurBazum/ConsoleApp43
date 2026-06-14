using Microsoft.Extensions.DependencyInjection;

namespace P43.Wpf.Infrastructure.Stores;
public static class StoreRegister
{
    public static IServiceCollection AddStores(this IServiceCollection services) => services.AddSingleton<INavigation, Navigation>();
}