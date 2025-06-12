using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;

namespace HotPotPlayer2.Controls;

public partial class MainSidebar : UserControl
{
    public MainSidebar()
    {
        InitializeComponent();
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
        AvaloniaProperty.Register<MainSidebar, string?>("SelectedPageName", coerce: OnSelectedPageNameChanged);

    private static string? OnSelectedPageNameChanged(AvaloniaObject a, string? newValue)
    {
        var sidbar = (MainSidebar)a;
        var newPageName = newValue;
        sidbar._selectedButton = newPageName switch
        {
            string n when n.StartsWith("Music") => sidbar.Music,
            string n when n.StartsWith("Video") => sidbar.Video,
            string n when n.StartsWith("Bilibili") => sidbar.Bilibili,
            string n when n.StartsWith("CloudMusic") => sidbar.CloudMusic,
            string n when n.StartsWith("Setting") => sidbar.Setting,
            _ => null,
        };
        return newValue;
    }

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
}