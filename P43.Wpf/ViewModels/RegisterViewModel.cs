using P43.Wpf.ViewModels.Base;
using System.Windows.Input;

namespace P43.Wpf.ViewModels;

public class RegisterViewModel : BaseViewModel
{
    public string Title
    {
        get => field;
        set => Set(ref field, value);
    } = "RegisterPage";

    private ICommand? _registerCmd;
}