using P43.Wpf.ViewModels.Base;
using P43.Wpf.Infrastructure.Stores;
using P43.Wpf.Infrastructure.Commands;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace P43.Wpf.ViewModels;
public class StartViewModel(INavigation navigation, IServiceProvider provider) : BaseViewModel
{
    private readonly INavigation _navigation = navigation;
    private readonly IServiceProvider _provider = provider;
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
        var vm = _provider.GetRequiredService<LoginViewModel>();
        _navigation.Next(vm);
    }

    private void ToRegisterViewCmdExecute(object? parameter)
    {
        var vm = _provider.GetRequiredService<RegisterViewModel>();
        _navigation.Next(vm);
    }
}