using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using HotPotPlayer2.ViewModels;

namespace HotPotPlayer2.Views.Pages;

public partial class Setting : UserControl
{
    public Setting()
    {
        InitializeComponent();
    }

    private void MusicLibrary_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
    }

    private void SuppressTap(object sender, TappedEventArgs e)
    {
        e.Handled = true;
    }
    private void AddJellyfinServerPopupOverlay_Tapped(object sender, TappedEventArgs e)
    {
        (DataContext as SettingPageViewModel)!.AddJellyfinServerPopupOverlayVisible = false;
    }
}