using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using HotPotPlayer2.ViewModels;

namespace HotPotPlayer2.Views.Pages;

public partial class Music : UserControl
{
    public Music()
    {
        InitializeComponent();
    }

    private void AlbumClick(object sender, RoutedEventArgs e)
    {
        ((MusicPageViewModel)DataContext!).AlbumClick(sender, e);
    }

}