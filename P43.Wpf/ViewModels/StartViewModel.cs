using P43.Wpf.ViewModels.Base;
using P43.Wpf.Infrastructure.Stores;
using P43.Wpf.Infrastructure.Commands;
using System.Windows.Input;

namespace P43.Wpf.ViewModels;
public class StartViewModel(INavigation navigation) : BaseViewModel
{
    private readonly INavigation _navigation = navigation;
    public string Title
    {
        get => field;
        set => Set(ref field, value);
    } = "StartPage";

    private ICommand? _toLoginViewCmd;
    private ICommand? _toRegisterViewCmd;

    public ICommand ToLoginViewCmd => _toLoginViewCmd ??= new RelayCommand(ToLoginViewCmdExecute, parameter => true);
    public ICommand ToRegisterViewCmd => _toRegisterViewCmd ??= new RelayCommand(ToRegisterViewCmdExecute, parameter => true);

    private void ToLoginViewCmdExecute(object? parameter)
    {
        _navigation.Next<LoginViewModel>();
    }

    private void ToRegisterViewCmdExecute(object? parameter)
    {
        _navigation.Next<RegisterViewModel>();
    }
}