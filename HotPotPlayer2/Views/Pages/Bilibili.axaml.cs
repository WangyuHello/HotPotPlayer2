using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HotPotPlayer2.ViewModels;

namespace HotPotPlayer2.Views.Pages;

public partial class Bilibili : UserControl
{
    public Bilibili()
    {
        InitializeComponent();
    }

    private void RefreshClick()
    {
        ((BilibiliPageViewModel)DataContext!).RefreshClick();
    }
}