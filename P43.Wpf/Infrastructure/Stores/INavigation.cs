using P43.Wpf.ViewModels.Base;
namespace P43.Wpf.Infrastructure.Stores;
public interface INavigation
{
    event Action? CurrentViewModelChanged;
    BaseViewModel CurrentViewModel { get; }
    void Next(BaseViewModel viewModel);
    void Previous();
}