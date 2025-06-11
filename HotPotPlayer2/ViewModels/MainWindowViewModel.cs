using CommunityToolkit.Mvvm.ComponentModel;
using HotPotPlayer2.Base;

namespace HotPotPlayer2.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public string Greeting { get; } = "Welcome to Avalonia!";

        [ObservableProperty]
        public partial int Test { get; set; }

        public void T()
        {
        }
    }
}
