using P43.Wpf.ViewModels.Base;
using System.Windows.Input;
using P43.Wpf.Infrastructure.Stores;
using P43.Wpf.Infrastructure.Commands;

namespace P43.Wpf.ViewModels;

public class LoginViewModel(INavigation navigation) : BaseViewModel
{
    private readonly INavigation _navigation = navigation;
    public string Title
    {
        get => field;
        set => Set(ref field, value);
    } = "LoginPage";

    public string UserName
    {
        get => field;
        set => Set(ref field, value);
    } = "Bob";

    private ICommand? _loginCmd;
    private ICommand? _backCmd;

    public ICommand BackCmd => _backCmd ??= new RelayCommand(BackCmdExecute, parameter => true);

    private void BackCmdExecute(object? parameter)
    {
        _navigation.Previous();
    }
}