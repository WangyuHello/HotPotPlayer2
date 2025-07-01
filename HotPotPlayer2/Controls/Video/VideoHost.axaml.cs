using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using HotPotPlayer2.ViewModels;

namespace HotPotPlayer2.Controls.Video;

public partial class VideoHost : UserControl
{
    public VideoHost()
    {
        InitializeComponent();
        DataContext = new VideoHostViewModel();
        (DataContext as VideoHostViewModel)?.VideoPlayer.GetNativeHost = GetNativeHost;
    }

    private nint GetNativeHost()
    {
        return Host.Hwnd;
    }
}