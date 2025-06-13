using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using System;
using System.Linq;

namespace HotPotPlayer2.Controls.Bilibili;

public partial class Header : UserControl
{
    public Header()
    {
        InitializeComponent();
    }

    public int SelectedIndex
    {
        get { return (int)GetValue(SelectedIndexProperty)!; }
        set { SetValue(SelectedIndexProperty, value); }
    }

    public static readonly AvaloniaProperty<int> SelectedIndexProperty =
        AvaloniaProperty.Register<Header, int>("SelectedIndex", coerce: OnSelectedIndexChanged);

    static int OnSelectedIndexChanged(AvaloniaObject sender, int ind)
    {
        ((Header)sender).SelectedIndexChanged(ind);
        return ind;
    }

    void SelectedIndexChanged(int ind)
    {
        var headers = HeaderContainer.Children;
        for (int i = 0; i < headers.Count; i++)
        {
            if (headers[i] is not ToggleButton h)
            {
                ind++;
                continue;
            }
            if (i == ind)
            {
                h.IsChecked = true;
            }
            else
            {
                h.IsChecked = false;
            }
        }
    }

    public event Action? OnRefreshClick;

    void RefreshClick(object sender, RoutedEventArgs e)
    {
        OnRefreshClick?.Invoke();
    }
    private void HeaderItemClick(object sender, RoutedEventArgs e)
    {
        var headers = HeaderContainer.Children;
        var b = sender as Control;
        var index = headers.Where(u => u is ToggleButton).ToList().IndexOf(b!);
        SelectedIndex = index;
        SelectedIndexChanged(index);
    }
}