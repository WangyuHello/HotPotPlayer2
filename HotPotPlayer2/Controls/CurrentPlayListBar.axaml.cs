using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using HotPotPlayer2.Base;
using HotPotPlayer2.ViewModels;
using Jellyfin.Sdk.Generated.Models;

namespace HotPotPlayer2.Controls;

public partial class CurrentPlayListBar : UserControl
{
    public CurrentPlayListBar()
    {
        InitializeComponent();
        DataContext = new CurrentPlayListBarViewModel();
    }

    private void RootTapped(object sender, TappedEventArgs e)
    {
        (DataContext as ViewModelBase)!.MusicPlayer.IsPlayListBarVisible = false;
    }

    private void BackClick(object sender, RoutedEventArgs e)
    {
        (DataContext as ViewModelBase)!.MusicPlayer.IsPlayListBarVisible = false;
    }

    private void InnerTapped(object sender, TappedEventArgs e)
    {
        e.Handled = true;
    }

    private void MusicItemClick(object sender, RoutedEventArgs e)
    {
        var music = (sender as Button)!.Tag as BaseItemDto;
        (DataContext as ViewModelBase)!.MusicPlayer.PlayNextContinue(music!);
    }
}