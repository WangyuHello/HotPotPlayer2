using Avalonia.Controls;
using HotPotPlayer2.ViewModels;

namespace HotPotPlayer2.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        void OnBackClick() { ((MainWindowViewModel)DataContext!).OnBackClick(); }

        void SelectedPageNameChanged(string name) { ((MainWindowViewModel)DataContext!).SelectedPageNameChanged(name); }
    }
}