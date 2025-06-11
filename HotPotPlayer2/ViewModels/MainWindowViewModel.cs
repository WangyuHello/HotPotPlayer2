using CommunityToolkit.Mvvm.ComponentModel;
using HotPotPlayer2.Base;
using HotPotPlayer2.Views.Pages;

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

            var videoVm = new VideoPageViewModel();
            var videoPage = new Video();
            videoVm.Page = videoPage;
            videoPage.DataContext = videoVm;

            Pages = [musicVm, videoVm];
            CurrentPage = Pages[0];
        }

        private readonly PageViewModelBase[] Pages;

        [ObservableProperty]
        public partial PageViewModelBase CurrentPage { get; set; }
        
    }
}
