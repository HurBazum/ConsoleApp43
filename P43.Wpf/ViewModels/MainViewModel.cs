using P43.Wpf.ViewModels.Base;
using P43.Wpf.Infrastructure.Stores;

namespace P43.Wpf.ViewModels;
public class MainViewModel : BaseViewModel
{
    private readonly INavigation _navigation;
    public string Title
    {
        get => field;
        set => Set(ref field, value);
    } = "Chatroom";

    public BaseViewModel ViewModel => _navigation.CurrentViewModel;

    public MainViewModel(INavigation navigation)
    {
        _navigation = navigation;
        _navigation.CurrentViewModelChanged += OnViewModelChanged;
    }

    private void OnViewModelChanged() => OnPropertyChanged(nameof(ViewModel));
}