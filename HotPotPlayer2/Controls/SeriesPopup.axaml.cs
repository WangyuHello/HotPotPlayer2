using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Transformation;
using ExCSS;
using HotPotPlayer2.Base;
using HotPotPlayer2.Models.Jellyfin;
using Jellyfin.Sdk.Generated.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HotPotPlayer2.Controls;

public partial class SeriesPopup : UserControl
{
    public SeriesPopup()
    {
        InitializeComponent();
        SeriesProperty.Changed.AddClassHandler<SeriesPopup>(SeriesChanged);
    }

    public BaseItemDto? Series
    {
        get { return (BaseItemDto?)GetValue(SeriesProperty); }
        set { SetValue(SeriesProperty, value); }
    }

    public static readonly AvaloniaProperty<BaseItemDto?> SeriesProperty =
        AvaloniaProperty.Register<SeriesPopup, BaseItemDto?>("Series");

    public BaseItemDto? SeriesInfo
    {
        get { return (BaseItemDto?)GetValue(SeriesInfoProperty); }
        set { SetValue(SeriesInfoProperty, value); }
    }

    public static readonly AvaloniaProperty<BaseItemDto?> SeriesInfoProperty =
        AvaloniaProperty.Register<SeriesPopup, BaseItemDto?>("SeriesInfo");

    public List<BaseItemDto>? Seasons
    {
        get { return (List<BaseItemDto>?)GetValue(SeasonsProperty); }
        set { SetValue(SeasonsProperty, value); }
    }

    public static readonly AvaloniaProperty<List<BaseItemDto>?> SeasonsProperty =
        AvaloniaProperty.Register<SeriesPopup, List<BaseItemDto>?>("Seasons");

    public List<BaseItemDto>? SelectedSeasonVideoItems
    {
        get { return (List<BaseItemDto>?)GetValue(SelectedSeasonVideoItemsProperty); }
        set { SetValue(SelectedSeasonVideoItemsProperty, value); }
    }

    public static readonly AvaloniaProperty<List<BaseItemDto>?> SelectedSeasonVideoItemsProperty =
        AvaloniaProperty.Register<SeriesPopup, List<BaseItemDto>?>("SelectedSeasonVideoItems");

    public List<CustomChapterInfo>? CustomChapters
    {
        get { return (List<CustomChapterInfo>?)GetValue(CustomChaptersProperty); }
        set { SetValue(CustomChaptersProperty, value); }
    }

    public static readonly AvaloniaProperty<List<CustomChapterInfo>?> CustomChaptersProperty =
        AvaloniaProperty.Register<SeriesPopup, List<CustomChapterInfo>?>("CustomChapters");

    public bool IsBackdropExpanded
    {
        get { return (bool)GetValue(IsBackdropExpandedProperty)!; }
        set { SetValue(IsBackdropExpandedProperty, value); }
    }

    public static readonly AvaloniaProperty<bool> IsBackdropExpandedProperty =
        AvaloniaProperty.Register<SeriesPopup, bool>("IsBackdropExpanded");

    private async void SeriesChanged(SeriesPopup popup, AvaloniaPropertyChangedEventArgs args)
    {
        if (args.NewValue is not BaseItemDto series) return;
        var app = Application.Current as AppBase;

        SeriesInfo = await app!.JellyfinMusicService.GetItemInfoAsync(series);
        SeasonSelector.Items.Clear();

        if (series.IsFolder!.Value)
        {
            //Series
            Seasons = await app.JellyfinMusicService.GetSeasonsAsync(series);
            int i = 0;
            foreach (var season in Seasons!)
            {
                SeasonSelector.Items.Add(new TabItem
                {
                    IsSelected = i == 0,
                    Header = season.Name,
                    FontSize = 16,
                    FontFamily = (FontFamily)app.Resources["MiSansRegular"]!
                });
                i++;
            }
            SelectedSeasonVideoItems = await app.JellyfinMusicService.GetEpisodes(Seasons[0]);
        }
        else
        {
            //Movie
            Seasons = null;
            SelectedSeasonVideoItems = null;

            CustomChapters = [.. SeriesInfo!.Chapters!.Select((c, i) => new CustomChapterInfo
            {
                ImageTag = c.ImageTag,
                Index = i,
                ParentId = SeriesInfo!.Id!.Value,
                Name = c.Name,
                StartPositionTicks = c.StartPositionTicks,
            })];
        }
    }

    private async void SeasonSelector_SelectionChanged(object sender, SelectionChangedEventArgs args)
    {
        var tab = sender as TabControl;
        if (tab!.SelectedItem is not TabItem selectedItem) { return; }
        int currentSelectedIndex = tab.Items.IndexOf(selectedItem);

        var season = Seasons![currentSelectedIndex];
        var app = Application.Current as AppBase;
        SelectedSeasonVideoItems = await app!.JellyfinMusicService.GetEpisodes(season);
    }

    private void BackdropExpand_Click(object sender, RoutedEventArgs e)
    {
        IsBackdropExpanded = !IsBackdropExpanded;
    }
    private void Backdrop_Tapped(object sender, TappedEventArgs e)
    {
        IsBackdropExpanded = !IsBackdropExpanded;
    }

    private void ItemClick(object sender, RoutedEventArgs e)
    {

    }
}

public static class SeriesPopupConverters
{
    public static FuncValueConverter<bool, string> GetBackdropExpandIcon = new(i =>
    {
        return i ? "\uE70E" : "\uE70D";
    });

    public static FuncValueConverter<BaseItemDto?, bool> GetMoviePlayVisible = new(i =>
    {
        if (i == null || i.IsFolder == null) return false;
        return !i.IsFolder.Value;
    });

    public static FuncValueConverter<BaseItemDto?, IEnumerable<string?>?> GetVideoStreams = new(i =>
    {
        return i?.MediaSources?.SelectMany(s => s.MediaStreams?.Where(m => m.Type == MediaStream_Type.Video)!)?.Select(s => s.DisplayTitle);
    });

    public static FuncValueConverter<BaseItemDto?, int> GetVideoStreamsSelectIndex = new(i =>
    {
        return 0;
    });

    public static FuncValueConverter<BaseItemDto?, IEnumerable<string?>?> GetAudioStreams = new(i =>
    {
        return i?.MediaStreams?.Where(m => m.Type == MediaStream_Type.Audio)?.Select(s => s.DisplayTitle);
    });

    public static FuncValueConverter<BaseItemDto?, int> GetAudioStreamsSelectIndex = new(i =>
    {
        return 0;
    });

    public static FuncValueConverter<List<BaseItemPerson>, IEnumerable<BaseItemPerson>?> GetDirector = new(i =>
    {
        return i?.Where(p => p.Type == BaseItemPerson_Type.Director);
    });

    public static FuncValueConverter<List<BaseItemPerson>, bool> GetDirectorVisible = new(i =>
    {
        return (i?.FirstOrDefault(p => p.Type == BaseItemPerson_Type.Director)) != null;
    });

    public static FuncValueConverter<List<BaseItemPerson>, IEnumerable<BaseItemPerson>?> GetWriter = new(i =>
    {
        return i?.Where(p => p.Type == BaseItemPerson_Type.Writer);
    });

    public static FuncValueConverter<BaseItemDto?, string?> GetWriterTitle = new(i =>
    {
        if (i == null)
        {
            return null;
        }
        return i.IsFolder!.Value ? "×÷Õß" : "±à¾ç";
    });

    public static FuncValueConverter<BaseItemDto?, bool> GetChapterVisible = new(i =>
    {
        if (i == null)
        {
            return false;
        }
        return (i.Chapters == null || i.Chapters.Count == 0) ? false : true;
    });

    static TransformOperations? _transY142;
    static TransformOperations? _transY284;
    static TransformOperations? _transY60;
    static TransformOperations? _transY0;

    static TransformOperations TransY142 => _transY142 ??= TransformOperations.Parse("translateY(142px)");
    static TransformOperations TransY284 => _transY284 ??= TransformOperations.Parse("translateY(284px)");
    static TransformOperations TransY60 => _transY60 ??= TransformOperations.Parse("translateY(60px)");
    static TransformOperations TransY0 => _transY0 ??= TransformOperations.Parse("translateY(0px)");

    public static FuncValueConverter<bool, TransformOperations> GetBackdropOffset = new(i =>
    {
        return i ? TransY142 : TransY0;
    });

    public static FuncValueConverter<bool, TransformOperations> GetMainInfoOffset = new(i =>
    {
        return i ? TransY284 : TransY0;
    });

    public static FuncValueConverter<bool, TransformOperations> GetLeftPanelOffset = new(i =>
    {
        return i ? TransY60 : TransY0;
    });
}