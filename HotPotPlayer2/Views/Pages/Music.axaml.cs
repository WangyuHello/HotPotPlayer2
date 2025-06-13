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

    private void AlbumGridView_ItemClick(object sender, RoutedEventArgs e)
    {
        ((MusicPageViewModel)DataContext!).AlbumGridView_ItemClick(sender, e);
    }
}