using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HotPotPlayer2.Base;
using Jellyfin.Sdk.Generated.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2.ViewModels
{
    public partial class SettingPageViewModel : PageViewModelBase
    {
        public override string? Name => "Setting";

        [ObservableProperty]
        public partial string? JellyfinUrl { get; set; }

        [ObservableProperty]
        public partial string? JellyfinUserName { get; set; }

        [ObservableProperty]
        public partial string? JellyfinPassword { get; set; }

        [ObservableProperty]
        public partial BaseItemDto SelectedMusicLibraryDto { get; set; }

        [ObservableProperty]
        public partial List<BaseItemDto> MusicLibraryDto { get; set; }

        [ObservableProperty]
        public partial bool EnableReplayGain { get; set; }

        partial void OnEnableReplayGainChanged(bool value)
        {
            Config.SetConfig(nameof(EnableReplayGain), value, true);
        }

        [ObservableProperty]
        public partial bool EnableRestorePrevLocation { get; set; }

        partial void OnEnableRestorePrevLocationChanged(bool value)
        {
            Config.SetConfig(nameof(EnableRestorePrevLocation), value, true);
        }

        [ObservableProperty]
        public partial string? MpvVersion { get; set; }

        private static string? GetMpvVersion()
        {
            return "";
        }

        public override async void OnNavigatedTo(object? args)
        {
            var info = await JellyfinMusicService.GetPublicSystemInfo();
            if (info == null) return;

            JellyfinUrl = info.LocalAddress;
            JellyfinUserName = Config.GetConfig<string>("JellyfinUserName");
            JellyfinPassword = Config.GetConfig<string>("JellyfinPassword");

            MusicLibraryDto = JellyfinMusicService.MusicLibraryDto;
            SelectedMusicLibraryDto = JellyfinMusicService.SelectedMusicLibraryDto;
            EnableReplayGain = Config.GetConfig(nameof(EnableReplayGain), true, true);
            EnableRestorePrevLocation = Config.GetConfig(nameof(EnableRestorePrevLocation), false, true);
            MpvVersion ??= GetMpvVersion();
        }

        private void OpenInstalledLocationClick(object sender, RoutedEventArgs e)
        {

        }

        [RelayCommand]
        public async Task OpenDataLocationClick()
        {
            var loc = Config.LocalFolder;
            var launcher = TopLevel.GetTopLevel(App.MainWindow)!.Launcher;
            await launcher.LaunchDirectoryInfoAsync(new DirectoryInfo(loc));
        }

        [RelayCommand]
        public async Task OpenConfigFileClick()
        {
            var loc = Path.Combine(Config.LocalFolder,"Config","Settings.json");
            var launcher = TopLevel.GetTopLevel(App.MainWindow)!.Launcher;
            await launcher.LaunchFileInfoAsync(new FileInfo(loc));
        }

        [RelayCommand]
        public async Task AddJellyfinServerClick()
        {
            if (string.IsNullOrEmpty(JellyfinUrl)||
                string.IsNullOrEmpty(JellyfinPassword)||
                string.IsNullOrEmpty(JellyfinUserName))
            {
                return;
            }

            var (info, msg) = await JellyfinMusicService.TryGetSystemInfoPublicAsync(JellyfinUrl);
            if (info == null)
            {
                return;
            }

            var (loginResult, message) = await JellyfinMusicService.TryLoginAsync(JellyfinUrl, JellyfinUserName, JellyfinPassword);

            if (!loginResult)
            {
                return;
            }

            Config.SetConfig("JellyfinUrl", JellyfinUrl);
            Config.SetConfig("JellyfinUserName", JellyfinUserName);
            Config.SetConfig("JellyfinPassword", JellyfinPassword);
            Config.SaveSettings();

            JellyfinMusicService.Reset();

            MusicLibraryDto = JellyfinMusicService.MusicLibraryDto;
            SelectedMusicLibraryDto = JellyfinMusicService.SelectedMusicLibraryDto;
        }
    }
}
