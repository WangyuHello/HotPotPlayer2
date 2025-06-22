using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HotPotPlayer2.Models;
using System;

namespace HotPotPlayer2.Controls;

public partial class Toast : UserControl
{
    public Toast()
    {
        InitializeComponent();
    }

    public event EventHandler? OnShow;
    public event EventHandler? OnHide;
    public void Show() { OnShow?.Invoke(this, new EventArgs()); }

    public void Hide() { OnHide?.Invoke(this, new EventArgs()); }

    public ToastInfo? ToastInfo
    {
        get { return (ToastInfo?)GetValue(ToastInfoProperty); }
        set { SetValue(ToastInfoProperty, value); }
    }

    public static readonly AvaloniaProperty<ToastInfo?> ToastInfoProperty =
        AvaloniaProperty.Register<Toast, ToastInfo?>("ToastInfo");
}