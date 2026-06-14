using P43.Wpf.ViewModels.Base;
using System.Windows.Input;

namespace P43.Wpf.ViewModels;

public class LoginViewModel : BaseViewModel
{
    public string Title
    {
        get => field;
        set => Set(ref field, value);
    } = "LoginPage";

    private ICommand? _loginCmd;
}