using P43.Wpf.ViewModels.Base;
using Microsoft.Extensions.DependencyInjection;

namespace P43.Wpf.Infrastructure.Stores;

public class Navigation(IServiceProvider provider) : INavigation
{
    private readonly IServiceProvider _provider = provider;
    public event Action? CurrentViewModelChanged;
    public Stack<BaseViewModel> ViewModels { get; private set; } = new();
    public BaseViewModel CurrentViewModel => ViewModels.Peek();
    public void Next<T>() where T : BaseViewModel
    {
        BaseViewModel viewModel = _provider.GetRequiredService<T>();
        ViewModels.Push(viewModel);
        OnCurrentViewModelChanged();
    }

    public void Previous()
    {
        if(ViewModels.Count > 1)
        {
            ViewModels.Pop();
            OnCurrentViewModelChanged();
        }
    }

    private void OnCurrentViewModelChanged() => CurrentViewModelChanged?.Invoke();
}