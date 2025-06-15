using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using HotPotPlayer2.Base;
using HotPotPlayer2.Extensions;
using HotPotPlayer2.Models;
using HotPotPlayer2.Service;
using HotPotPlayer2.ViewModels;
using Jellyfin.Sdk.Generated.Models;
using System;
using System.Linq;

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
        (DataContext as PlayBarViewModel)?.PlayButtonClick(sender, e);
    }
    private void PlayModeButtonClick(object sender, RoutedEventArgs e)
    {
        (DataContext as PlayBarViewModel)?.PlayModeButtonClick(sender, e);
    }
    private void PlayPreviousButtonClick(object sender, RoutedEventArgs e)
    {
        (DataContext as PlayBarViewModel)?.PlayPreviousButtonClick(sender, e);
    }
    private void PlayNextButtonClick(object sender, RoutedEventArgs e)
    {
        (DataContext as PlayBarViewModel)?.PlayNextButtonClick(sender, e);
    }

    private void PlaySlider_DragStarted(object sender, VectorEventArgs e)
    {
        (DataContext as PlayBarViewModel)?.PlaySlider_DragStarted(sender, e);
    }

    private void PlaySlider_DragCompleted(object sender, VectorEventArgs e)
    {
        (DataContext as PlayBarViewModel)?.PlaySlider_DragCompleted(PlaySlider.Value);
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

    const string Loop = "\uE1CD";
    const string SingleLoop = "\uE1CC";
    const string Shuffle = "\uE8B1";
    const string Single = "\uE776";

    public static FuncValueConverter<PlayMode, string> GetPlayModeIcon = new(i =>
    {
        return i switch
        {
            PlayMode.Loop => Loop,
            PlayMode.SingleLoop => SingleLoop,
            PlayMode.Shuffle => Shuffle,
            PlayMode.Single => Single,
            _ => Single,
        };
    });

    public static FuncMultiValueConverter<TimeSpan?, double> GetSliderValue = new(i =>
    {
        var times = i.ToArray();
        var current = times[0];
        var total = times[1];

        if (total == null)
        {
            return 0;
        }
        else if (total.Value.Ticks == 0)
        {
            return 0;
        }
        return 100 * (current?.Ticks ?? 0) / ((TimeSpan)total).Ticks;
    });
}