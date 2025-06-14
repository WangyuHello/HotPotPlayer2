using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using HotPotPlayer2.Base;
using HotPotPlayer2.Extensions;
using HotPotPlayer2.Service;
using HotPotPlayer2.ViewModels;
using Jellyfin.Sdk.Generated.Models;
using System;

namespace HotPotPlayer2.Controls;

public partial class PlayBar : UserControl
{
    public PlayBar()
    {
        InitializeComponent();
        DataContext = new PlayBarViewModel();
    }

    private void PlayButtonClick(object sender, RoutedEventArgs e)
    {
        ((PlayBarViewModel)DataContext!).PlayButtonClick(sender, e);
    }
}
public static class PlayBarConverters
{
    private static JellyfinMusicService JellyfinMusicService => ((IServiceLocator)Application.Current!).JellyfinMusicService;

    public static FuncValueConverter<BaseItemDto, string> GetSubtitle = new(i =>
    {
        if (i == null) return string.Empty;
        return $"{i.GetJellyfinArtists()} ¡¤ {i.Album}";
    });

    public static FuncValueConverter<BaseItemDto, Uri?> GetPlayBarImage = new(i =>
    {
        return JellyfinMusicService.GetPrimaryJellyfinImageWidth(i?.ImageTags, i?.Id, 100);
    });

    public static FuncValueConverter<bool, string> GetPlayButtonIcon = new(i =>
    {
        return i ? "\uE769" : "\uE768";
    });
}