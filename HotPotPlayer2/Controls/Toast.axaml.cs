using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HotPotPlayer2.Models;

namespace HotPotPlayer2.Controls;

public partial class Toast : UserControl
{
    public Toast()
    {
        InitializeComponent();
    }

    public ToastInfo? ToastInfo
    {
        get { return (ToastInfo?)GetValue(ToastInfoProperty); }
        set { SetValue(ToastInfoProperty, value); }
    }

    public static readonly AvaloniaProperty<ToastInfo?> ToastInfoProperty =
        AvaloniaProperty.Register<Toast, ToastInfo?>("ToastInfo");
}