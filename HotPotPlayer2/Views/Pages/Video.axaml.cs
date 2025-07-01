using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using HotPotPlayer2.ViewModels;

namespace HotPotPlayer2.Views.Pages;

public partial class Video : UserControl
{
    public Video()
    {
        InitializeComponent();
    }

    private void SeriesClick(object sender, RoutedEventArgs e)
    {
        (DataContext as VideoPageViewModel)?.SeriesClick(sender, e);
    }

    private void PlaySeriesClick(object sender, RoutedEventArgs e)
    {
        (DataContext as VideoPageViewModel)?.PlaySeriesClick(sender, e);
        e.Handled = true;
    }

    private void SuppressTap(object sender, TappedEventArgs e)
    {
        e.Handled = true;
    }

    private void SeriesPopupOverlay_Tapped(object sender, TappedEventArgs e)
    {
        (DataContext as VideoPageViewModel)!.SeriesPopupOverlayVisible = false;
    }
}