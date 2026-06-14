using P43.Wpf.ViewModels.Base;
using P43.Wpf.ViewModels;

namespace P43.Wpf.Infrastructure.Stores;

public class Navigation : INavigation
{
    public event Action? CurrentViewModelChanged;
    public Stack<BaseViewModel> ViewModels { get; private set; } = [];
    public BaseViewModel CurrentViewModel => ViewModels.Peek();
    public void Next(BaseViewModel viewModel)
    {
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

    public Navigation(StartViewModel svm)
    {
        ViewModels.Push(svm);
    }
}