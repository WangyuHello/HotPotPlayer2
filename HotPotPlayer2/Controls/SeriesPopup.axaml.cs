using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
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
}