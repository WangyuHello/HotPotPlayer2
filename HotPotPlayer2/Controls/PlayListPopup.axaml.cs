using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Jellyfin.Sdk.Generated.Models;
using System;
using System.Collections.Generic;

namespace HotPotPlayer2.Controls;

public partial class PlayListPopup : UserControl
{
    public PlayListPopup()
    {
        InitializeComponent();
    }

    public event EventHandler? OnShow;
    public event EventHandler? OnHide;
    public void Show() { OnShow?.Invoke(this, new EventArgs()); }
    public void Hide() { OnHide?.Invoke(this, new EventArgs()); }

    public BaseItemDto? PlayList
    {
        get { return (BaseItemDto?)GetValue(PlayListProperty); }
        set { SetValue(PlayListProperty, value); }
    }

    public static readonly AvaloniaProperty<BaseItemDto?> PlayListProperty =
        AvaloniaProperty.Register<PlayListPopup, BaseItemDto?>("PlayList");

    public List<BaseItemDto>? PlayListMusicItems
    {
        get { return (List<BaseItemDto>?)GetValue(PlayListMusicItemsProperty); }
        set { SetValue(PlayListMusicItemsProperty, value); }
    }

    public static readonly AvaloniaProperty<List<BaseItemDto>?> PlayListMusicItemsProperty =
        AvaloniaProperty.Register<PlayListPopup, List<BaseItemDto>?>("PlayListMusicItems");

    public BaseItemDto? PlayListInfo
    {
        get { return (BaseItemDto?)GetValue(PlayListInfoProperty); }
        set { SetValue(PlayListInfoProperty, value); }
    }

    public static readonly AvaloniaProperty<BaseItemDto?> PlayListInfoProperty =
        AvaloniaProperty.Register<PlayListPopup, BaseItemDto?>("PlayListInfo");

    public event Action<List<BaseItemDto>?>? OnPlayListPlayClick;
    public event Action<BaseItemDto, List<BaseItemDto>?>? OnMusicPlayClick;

    private void PlayListPlayClick(object sender, RoutedEventArgs e)
    {
        OnPlayListPlayClick?.Invoke(PlayListMusicItems);
        e.Handled = true;
    }

    private void ItemClick(object sender, RoutedEventArgs e)
    {
        var item = (sender as Button)!.Tag as BaseItemDto;
        if (item != null && PlayList != null)
        {
            OnMusicPlayClick?.Invoke(item, PlayListMusicItems);
        }
    }
}