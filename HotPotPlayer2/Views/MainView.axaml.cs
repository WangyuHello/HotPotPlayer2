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

    public string? GetSavePageName() => (DataContext as MainWindowViewModel)?.GetSavePageName();

    void OnBackClick() { (DataContext as MainWindowViewModel)!.OnBackClick(); }

    void SelectedPageNameChanged(string name) { (DataContext as MainWindowViewModel)!.SelectedPageNameChanged(name); }
}

public static class MainViewConverters
{
    public static FuncValueConverter<bool, double> GetToastTranslation = new(v =>
    {
        return v ? 0 : -120;
    });
}