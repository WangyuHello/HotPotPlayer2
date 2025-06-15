using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HotPotPlayer2.ViewModels;

namespace HotPotPlayer2.Controls;

public partial class CurrentPlayListBar : UserControl
{
    public CurrentPlayListBar()
    {
        InitializeComponent();
        DataContext = new CurrentPlayListBarViewModel();
    }
}