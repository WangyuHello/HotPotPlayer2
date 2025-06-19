using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using HotPotPlayer2.ViewModels;

namespace HotPotPlayer2.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    void OnBackClick() { ((MainWindowViewModel)DataContext!).OnBackClick(); }

    void SelectedPageNameChanged(string name) { ((MainWindowViewModel)DataContext!).SelectedPageNameChanged(name); }
}

public static class MainViewConverters
{
    public static FuncValueConverter<bool, double> GetToastTranslation = new(v =>
    {
        return v ? 0 : -120;
    });
}