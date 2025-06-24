using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using HotPotPlayer2.Service;
using HotPotPlayer2.ViewModels;
using System.ComponentModel;

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
    public static FuncValueConverter<bool, double> GetOpacity = new(i =>
    {
        return i ? 1 : 0;
    });
}