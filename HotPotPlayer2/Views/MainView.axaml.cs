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
        PropertyChanged += MainView_PropertyChanged;
    }

    private void MainView_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == DataContextProperty)
        {
            (DataContext as MainWindowViewModel)!.MusicPlayer.PropertyChanged += MusicPlayer_PropertyChanged;
        }
    }

    public string? GetSavePageName() => (DataContext as MainWindowViewModel)?.GetSavePageName();

    void OnBackClick() { (DataContext as MainWindowViewModel)!.OnBackClick(); }

    void SelectedPageNameChanged(string name) { (DataContext as MainWindowViewModel)!.SelectedPageNameChanged(name); }
}

public static class MainViewConverters
{
    public static FuncValueConverter<bool, TranslateTransform> GetToastTranslation = new(v =>
    {
        return new TranslateTransform(0, v ? 0 : -120);
    });
}