using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Jellyfin.Sdk.Generated.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HotPotPlayer2.Controls;

public partial class MainSidebar : UserControl
{
    public MainSidebar()
    {
        InitializeComponent();
        SelectedPageNameProperty.Changed.AddClassHandler<MainSidebar>(OnSelectedPageNameChanged);
        //ShowPlayBar.PropertyChanged += ShowPlayBar_PropertyChanged;
        //cancellationTokenSource = new CancellationTokenSource();
    }

    public bool IsBackEnable
    {
        get { return (bool)GetValue(IsBackEnableProperty)!; }
        set { SetValue(IsBackEnableProperty, value); }
    }

    public static readonly AvaloniaProperty<bool> IsBackEnableProperty =
        AvaloniaProperty.Register<MainSidebar, bool>("IsBackEnable");


    public string? SelectedPageName
    {
        get { return (string?)GetValue(SelectedPageNameProperty); }
        set { SetValue(SelectedPageNameProperty, value); }
    }

    public static readonly AvaloniaProperty<string?> SelectedPageNameProperty =
        AvaloniaProperty.Register<MainSidebar, string?>("SelectedPageName");

    private void OnSelectedPageNameChanged(MainSidebar a, AvaloniaPropertyChangedEventArgs args)
    {
        var sidbar = a;
        var newPageName = args.NewValue;
        sidbar._selectedButton = newPageName switch
        {
            string n when n.StartsWith("Music") => sidbar.Music,
            string n when n.StartsWith("Video") => sidbar.Video,
            string n when n.StartsWith("Bilibili") => sidbar.Bilibili,
            string n when n.StartsWith("CloudMusic") => sidbar.CloudMusic,
            string n when n.StartsWith("Setting") => sidbar.Setting,
            _ => null,
        };
    }

    public ICommand? ShowPlayBarCommand
    {
        get { return (ICommand?)GetValue(ShowPlayBarCommandProperty); }
        set { SetValue(ShowPlayBarCommandProperty, value); }
    }

    public static readonly AvaloniaProperty<ICommand?> ShowPlayBarCommandProperty =
        AvaloniaProperty.Register<MainSidebar, ICommand?>("ShowPlayBarCommand");

    public BaseItemDto? CurrentPlaying
    {
        get { return (BaseItemDto?)GetValue(CurrentPlayingProperty)!; }
        set { SetValue(CurrentPlayingProperty, value); }
    }

    public static readonly AvaloniaProperty<BaseItemDto?> CurrentPlayingProperty =
        AvaloniaProperty.Register<MainSidebar, BaseItemDto?>("CurrentPlaying");

    public bool IsPlayBarVisible
    {
        get { return (bool)GetValue(IsPlayBarVisibleProperty)!; }
        set { SetValue(IsPlayBarVisibleProperty, value); }
    }

    public static readonly AvaloniaProperty<bool> IsPlayBarVisibleProperty =
        AvaloniaProperty.Register<MainSidebar, bool>("IsPlayBarVisible");

    Button? _selectedButton;
    public event Action<string>? SelectedPageNameChanged;
    public event Action? OnBackClick;

    private void BackClick(object sender, RoutedEventArgs e)
    {
        OnBackClick?.Invoke();
    }

    private void NavigateClick(object sender, RoutedEventArgs e)
    {
        var b = (Button)sender;
        var name = (string)b.Tag!;
        SelectedPageNameChanged?.Invoke(name);
        SelectedPageName = name;
    }

    private void ShowPlayBarClick(object sender, RoutedEventArgs e)
    {
        if (ShowPlayBarCommand?.CanExecute(null) ?? false)
        {
            ShowPlayBarCommand.Execute(null);
        }
    }

    //Animation? rotateAni;
    //readonly CancellationTokenSource cancellationTokenSource;
    //private void ShowPlayBar_PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    //{
    //    if (e.Property == IsVisibleProperty)
    //    {
    //        var visible = (bool?)e.NewValue ?? false;
    //        if (visible)
    //        {
    //            rotateAni ??= (Animation)Resources["RotateAnimation"]!;
    //            // https://github.com/AvaloniaUI/Avalonia/discussions/16757
    //            _ = rotateAni.RunAsync(RotateBorder, cancellationTokenSource.Token);
    //        }
    //        else
    //        {
    //            cancellationTokenSource.Cancel();
    //        }
    //    }
    //}
}

public static class MainSidebarConverters
{
    public static FuncMultiValueConverter<object?, bool> GetShowPlayBarVisible = new(os =>
    {
        var osa = os.ToArray();
        var playbarVisible = osa[0] as bool?;

        if (osa[1] is not BaseItemDto currentPlaying) return false;
        return !(playbarVisible ?? false);
    });
}