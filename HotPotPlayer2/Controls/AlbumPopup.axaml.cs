using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Jellyfin.Sdk.Generated.Models;
using System;
using System.Collections.Generic;

namespace HotPotPlayer2.Controls;

public partial class AlbumPopup : UserControl
{
    public AlbumPopup()
    {
        InitializeComponent();
    }

    public event EventHandler? OnShow;
    public event EventHandler? OnHide;
    public void Show() { OnShow?.Invoke(this, new EventArgs()); }
    public void Hide() { OnHide?.Invoke(this, new EventArgs()); }

    public BaseItemDto? Album
    {
        get { return (BaseItemDto?)GetValue(AlbumProperty); }
        set { SetValue(AlbumProperty, value); }
    }

    public static readonly AvaloniaProperty<BaseItemDto?> AlbumProperty =
        AvaloniaProperty.Register<AlbumPopup, BaseItemDto?>("Album");

    public List<BaseItemDto>? AlbumMusicItems
    {
        get { return (List<BaseItemDto>?)GetValue(AlbumMusicItemsProperty); }
        set { SetValue(AlbumMusicItemsProperty, value); }
    }

    public static readonly AvaloniaProperty<List<BaseItemDto>?> AlbumMusicItemsProperty =
        AvaloniaProperty.Register<AlbumPopup,List<BaseItemDto>?>("AlbumMusicItems");

    public BaseItemDto? AlbumInfo
    {
        get { return (BaseItemDto?)GetValue(AlbumInfoProperty); }
        set { SetValue(AlbumInfoProperty, value); }
    }

    public static readonly AvaloniaProperty<BaseItemDto?> AlbumInfoProperty =
        AvaloniaProperty.Register<AlbumPopup, BaseItemDto?>("AlbumInfo");

    public event EventHandler<RoutedEventArgs>? OnAlbumPlayClick;
    public event Action<BaseItemDto, BaseItemDto>? OnMusicPlayClick;

    private void AlbumPlayClick(object sender, RoutedEventArgs e)
    {
        OnAlbumPlayClick?.Invoke(sender, e);
    }

    private void ItemClick(object sender, RoutedEventArgs e)
    {
        var item = (sender as Button)!.Tag as BaseItemDto;
        if (item != null && Album != null)
        {
            OnMusicPlayClick?.Invoke(item, Album);
        }
    }
}