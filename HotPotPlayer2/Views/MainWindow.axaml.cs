using Avalonia.Controls;
using HotPotPlayer2.Models;
using HotPotPlayer2.ViewModels;

namespace HotPotPlayer2.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void ShowToast(ToastInfo toast)
        {
            MainView.ShowToast(toast);
        }
    }
}