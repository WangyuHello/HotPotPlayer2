using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HotPotPlayer2.ViewModels;

namespace HotPotPlayer2.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    void OnBackClick() { ((MainWindowViewModel)DataContext!).OnBackClick(); }

    void SelectedPageNameChanged(string name) { ((MainWindowViewModel)DataContext!).SelectedPageNameChanged(name); }
}