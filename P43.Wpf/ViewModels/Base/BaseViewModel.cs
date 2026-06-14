using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace P43.Wpf.ViewModels.Base;

public class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName]string? propertName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertName));
    protected bool Set<T>(ref T field, T value, [CallerMemberName]string? propertyName = null)
    {
        if(Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

}