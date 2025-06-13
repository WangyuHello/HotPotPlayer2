using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;

namespace HotPotPlayer2.Controls.Bilibili;

public partial class Header : UserControl
{
    public Header()
    {
        InitializeComponent();
    }

    public int? SelectedIndex
    {
        get { return (int?)GetValue(SelectedIndexProperty); }
        set { SetValue(SelectedIndexProperty, value); }
    }

    public static readonly AvaloniaProperty<int?> SelectedIndexProperty =
        AvaloniaProperty.Register<Header, int?>("SelectedIndex");

    public event Action? OnRefreshClick;
}