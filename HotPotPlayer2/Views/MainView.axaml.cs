using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using HotPotPlayer2.Models;
using HotPotPlayer2.Service;
using HotPotPlayer2.ViewModels;
using System.ComponentModel;

namespace HotPotPlayer2.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    public string? GetSavePageName() => (DataContext as MainWindowViewModel)?.GetSavePageName();

    void OnBackClick() { (DataContext as MainWindowViewModel)!.OnBackClick(); }

    void SelectedPageNameChanged(string name) { (DataContext as MainWindowViewModel)!.SelectedPageNameChanged(name); }
}

public static class MainViewConverters
{
    public static FuncValueConverter<bool, double> GetOpacity = new(i =>
    {
        return i ? 1 : 0;
    });

    public static FuncValueConverter<VideoPlayVisualState, HorizontalAlignment> GetVideoPlayHorizontalAlignment = new(i =>
    {
        return i switch
        {
            VideoPlayVisualState.TinyHidden => HorizontalAlignment.Left,
            VideoPlayVisualState.FullHost => HorizontalAlignment.Stretch,
            VideoPlayVisualState.FullWindow => HorizontalAlignment.Stretch,
            VideoPlayVisualState.FullScreen => HorizontalAlignment.Stretch,
            VideoPlayVisualState.SmallHost => HorizontalAlignment.Right,
            _ => HorizontalAlignment.Stretch,
        };
    });

    public static FuncValueConverter<VideoPlayVisualState, VerticalAlignment> GetVideoPlayVerticalAlignment = new(i =>
    {
        return i switch
        {
            VideoPlayVisualState.TinyHidden => VerticalAlignment.Bottom,
            VideoPlayVisualState.FullHost => VerticalAlignment.Stretch,
            VideoPlayVisualState.FullWindow => VerticalAlignment.Stretch,
            VideoPlayVisualState.FullScreen => VerticalAlignment.Stretch,
            VideoPlayVisualState.SmallHost => VerticalAlignment.Bottom,
            _ => VerticalAlignment.Stretch,
        };
    });

    public static FuncValueConverter<VideoPlayVisualState, Thickness> GetVideoPlayMargin = new(i =>
    {
        return i switch
        {
            VideoPlayVisualState.TinyHidden => new Thickness(4, 0, 0, 4),
            VideoPlayVisualState.FullHost => new Thickness(0, 0, 0, 0),
            VideoPlayVisualState.FullWindow => new Thickness(0, 0, 0, 0),
            VideoPlayVisualState.FullScreen => new Thickness(0, 0, 0, 0),
            VideoPlayVisualState.SmallHost => new Thickness(0, 0, 16, 16),
            _ => new Thickness(0, 0, 0, 0),
        };
    });

    const double _smallWindowWidth = 400;
    const double _tinyWindowWidth = 0;

    public static FuncValueConverter<VideoPlayVisualState, double> GetVideoPlayWidth = new(i =>
    {
        return i switch
        {
            VideoPlayVisualState.TinyHidden => _tinyWindowWidth,
            VideoPlayVisualState.FullHost => double.NaN,
            VideoPlayVisualState.FullWindow => double.NaN,
            VideoPlayVisualState.FullScreen => double.NaN,
            VideoPlayVisualState.SmallHost => _smallWindowWidth,
            _ => double.NaN,
        };
    });

    public static FuncValueConverter<VideoPlayVisualState, double> GetVideoPlayHeight = new(i =>
    {
        return i switch
        {
            VideoPlayVisualState.TinyHidden => (_tinyWindowWidth * 9) / 16,
            VideoPlayVisualState.FullHost => double.NaN,
            VideoPlayVisualState.FullWindow => double.NaN,
            VideoPlayVisualState.FullScreen => double.NaN,
            VideoPlayVisualState.SmallHost => (_smallWindowWidth * 9) / 16,
            _ => double.NaN,
        };
    });
}