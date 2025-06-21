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
        public MainWindowViewModel(string initPage)
        {
            InitPage = initPage;
            SelectedPageNameChanged(InitPage);
            SelectedPageName = InitPage;
        }

        private readonly Dictionary<string, PageViewModelBase> PageCache = [];
        private readonly Stack<PageViewModelBase> NavigationStack = new();
        private readonly string InitPage;

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
            var has = PageCache.TryGetValue(name, out var selectVm);
            if (!has)
            {
                switch (name)
                {
                    case "Music":
                        var musicVm = new MusicPageViewModel();
                        var musicPage = new Music();
                        musicVm.Page = musicPage;
                        musicPage.DataContext = musicVm;
                        selectVm = musicVm;
                        PageCache.Add(name, selectVm);
                        break;

                    case "Video":
                        var videoVm = new VideoPageViewModel();
                        var videoPage = new Video();
                        videoVm.Page = videoPage;
                        videoPage.DataContext = videoVm;
                        selectVm = videoVm;
                        PageCache.Add(name, selectVm);
                        break;

                    case "Bilibili":
                        var bilibiliVm = new BilibiliPageViewModel();
                        var bilibiliPage = new Views.Pages.Bilibili();
                        bilibiliVm.Page = bilibiliPage;
                        bilibiliPage.DataContext = bilibiliVm;
                        selectVm = bilibiliVm;
                        PageCache.Add(name, selectVm);
                        break;

                    case "CloudMusic":
                        var cloudMusicVm = new CloudMusicPageViewModel();
                        var cloudMusicPage = new CloudMusic();
                        cloudMusicVm.Page = cloudMusicPage;
                        cloudMusicPage.DataContext = cloudMusicVm;
                        selectVm = cloudMusicVm;
                        PageCache.Add(name, selectVm);
                        break;

                    case "Setting":
                        var settingVm = new SettingPageViewModel();
                        var settingPage = new Setting();
                        settingVm.Page = settingPage;
                        settingPage.DataContext = settingVm;
                        selectVm = settingVm;
                        PageCache.Add(name, selectVm);
                        break;
                    default:
                        break;
                }
            }
            if (selectVm != null)
            {
                CurrentPage = selectVm;
                NavigationStack.Push(selectVm);
                if (NavigationStack.Count > 1)
                {
                    IsBackEnable = true;
                }
                CurrentPage.OnNavigatedTo(null);
            }
        }

        public string? GetSavePageName()
        {
            if (SelectedPageName == null)
            {
                return null;
            }
            var segs = SelectedPageName.Split(".");
            var mainName = segs[0].Replace("Sub", "");
            if (mainName == "VideoPlay" || mainName == "BiliVideoPlay")
            {
                mainName = null; // Do not save
            }
            return mainName;
        }
    }
}
