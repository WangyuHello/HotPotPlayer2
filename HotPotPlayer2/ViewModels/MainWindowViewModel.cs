using CommunityToolkit.Mvvm.ComponentModel;
using HotPotPlayer2.Base;

namespace HotPotPlayer2.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            var musicVm = new MusicPageViewModel();
            var musicPage = new Music();
            musicVm.Page = musicPage;
            musicPage.DataContext = musicVm;

            Pages = [musicVm];
            CurrentPage = Pages[0];
        }

        private readonly PageViewModelBase[] Pages;

        [ObservableProperty]
        public partial PageViewModelBase CurrentPage { get; set; }
        
    }
}
