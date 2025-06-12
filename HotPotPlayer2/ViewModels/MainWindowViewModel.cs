using CommunityToolkit.Mvvm.ComponentModel;
using HotPotPlayer2.Base;
using HotPotPlayer2.Views.Pages;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

            var bilibiliVm = new BilibiliPageViewModel();
            var bilibiliPage = new Views.Pages.Bilibili();
            bilibiliVm.Page = bilibiliPage;
            bilibiliPage.DataContext = bilibiliVm;

            var cloudMusicVm = new CloudMusicPageViewModel();
            var cloudMusicPage = new CloudMusic();
            cloudMusicVm.Page = cloudMusicPage;
            cloudMusicPage.DataContext = cloudMusicVm;

            var settingVm = new SettingPageViewModel();
            var settingPage = new Setting();
            settingVm.Page = settingPage;
            settingPage.DataContext = settingVm;

            Pages = [musicVm, videoVm, bilibiliVm, cloudMusicVm, settingVm];
        }

        private readonly PageViewModelBase[] Pages;
        private readonly Stack<PageViewModelBase> NavigationStack = new();

        [ObservableProperty]
        public partial PageViewModelBase? CurrentPage { get; set; }

        [ObservableProperty]
        public partial string? SelectedPageName { get; set; }

        [ObservableProperty]
        public partial bool IsBackEnable { get; set; }

        public void OnBackClick() 
        {
            if (NavigationStack.Count >= 1)
            {
                var top = NavigationStack.Pop();
                SelectedPageName = top.Name;
                CurrentPage = top;
                if(NavigationStack.Count == 0) { IsBackEnable = false; }
            }
        }

        public void SelectedPageNameChanged(string name) 
        {
            var selectVm = Pages.FirstOrDefault(p => p.Name == name);
            if (selectVm != null)
            {
                CurrentPage = selectVm;
                NavigationStack.Push(selectVm);
                IsBackEnable = true;
            }
        }
    }
}
