using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using HotPotPlayer2.ViewModels;
using Jellyfin.Sdk.Generated.Models;

namespace HotPotPlayer2.Views.Pages;

public partial class Music : UserControl
{
    public Music()
    {
        InitializeComponent();
    }

    private void AlbumClick(object sender, RoutedEventArgs e)
    {
        (DataContext as MusicPageViewModel)!.AlbumClick(sender, e);
    }

    private void PlayAlbumClick(object sender, RoutedEventArgs e)
    {
        (DataContext as MusicPageViewModel)!.PlayAlbumClick(sender, e);
        e.Handled = true;
    }

    private void MusicItemClick(BaseItemDto music, BaseItemDto album)
    {
        (DataContext as MusicPageViewModel)!.MusicItemClick(music, album);
    }


    private void AlbumPopupOverlay_Tapped(object sender, TappedEventArgs e)
    {
        (DataContext as MusicPageViewModel)!.AlbumPopupOverlayVisible = false;
    }

    private void SuppressTap(object sender, TappedEventArgs e)
    {
        e.Handled = true;
    }

    private void TabSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.RemovedItems.Count != 0)
        {
            switch ((e.AddedItems[0] as TabItem)?.Header)
            {
                case "²¥·ÅÁÐ±í":
                    (DataContext as MusicPageViewModel)!.JellyfinPlayListListLoadMore();
                    break;
                default:
                    break;
            }
        }
    }

    private void OnAlbumListScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (sender is ScrollViewer scrollViewer)
        {
            if (scrollViewer.Offset.Y >= (scrollViewer.ScrollBarMaximum.Y - 50) && scrollViewer.ScrollBarMaximum.Y > 0)
            {
                (DataContext as MusicPageViewModel)!.JellyfinAlbumListLoadMore();
            }
        }
    }
    private void OnPlayListListScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (sender is ScrollViewer scrollViewer)
        {
            if (scrollViewer.Offset.Y >= (scrollViewer.ScrollBarMaximum.Y - 50) && scrollViewer.ScrollBarMaximum.Y > 0)
            {
                (DataContext as MusicPageViewModel)!.JellyfinPlayListListLoadMore();
            }
        }
    }
}

public static class MusicConverters
{
    public static FuncValueConverter<bool, double> GetPopupOpacity = new(b =>
    {
        return b ? 1 : 0;
    });

    public static FuncValueConverter<bool, double> GetPopupScale = new(b =>
    {
        return b ? 1 : 0.8;
    });
}