using CommunityToolkit.Mvvm.ComponentModel;
using HotPotPlayer2.Base;
using HotPotPlayer2.Extensions;
using Jellyfin.Sdk.Generated.Models;
using Mpv.NET.Player;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WINDOWS
using Windows.Media;
#endif

namespace HotPotPlayer2.Service
{
    public partial class MusicPlayerService(ConfigBase config, AppBase app) : PlayerService(config, app)
    {
        [ObservableProperty]
        public partial bool IsPlayBarVisible { get; set; }

        [ObservableProperty]
        public partial bool IsPlayListBarVisible { get; set; }

        [ObservableProperty]
        public partial bool IsPlayScreenVisible { get; set; }

        public bool EnableReplayGain => Config.GetConfig("EnableReplayGain", true, true);
        public bool SuppressTogglePlayListBar { get; set; }

        public void TogglePlayListBarVisibility()
        {
            if (!SuppressTogglePlayListBar || IsPlayScreenVisible)
            {
                IsPlayListBarVisible = !IsPlayListBarVisible;
            }
        }

        public override async void PlayNext(BaseItemDto music)
        {
            if (music.Type == BaseItemDto_Type.MusicAlbum)
            {
                // Album
                var albumItems = await App.JellyfinMusicService.GetAlbumMusicItemsAsync(music);
                CurrentPlayList = [.. albumItems!];
                PlayNext(0);
            }
            else if (music.Type == BaseItemDto_Type.Playlist)
            {

            }
            else
            {
                // Single Music
                CurrentPlayList = [music];
                PlayNext(0);
            }
        }

        public async void PlayNext(BaseItemDto music, BaseItemDto album)
        {
            if (music.Type == BaseItemDto_Type.Playlist)
            {

            }
            else
            {
                var albumItems = await App.JellyfinMusicService.GetAlbumMusicItemsAsync(album);
                CurrentPlayList = [.. albumItems!];
                PlayNext(music.IndexNumber - 1);
            }
        }

        public void PlayNext(BaseItemDto music, IEnumerable<BaseItemDto> list)
        {
            CurrentPlayList = [.. list];
            var index = CurrentPlayList.IndexOf(music);
            PlayNext(index);
        }

        public void PlayNext(int index, IEnumerable<BaseItemDto> list)
        {
            CurrentPlayList = new ObservableCollection<BaseItemDto>(list);
            PlayNext(index);
        }
        public void PlayNext(IEnumerable<BaseItemDto> list)
        {
            PlayNext(0, list);
        }

        public void PlayNextContinue(BaseItemDto music)
        {
            var index = CurrentPlayList?.IndexOf(music);
            PlayNext(index);
        }

        public void AddToPlayListLast(BaseItemDto music)
        {
            CurrentPlayList?.Add(music);
        }

        public void AddToPlayListNext(BaseItemDto music)
        {
            CurrentPlayList?.Insert(CurrentPlayingIndex + 1, music);
        }

        protected override void SetupMpvInitProperty(MpvPlayer mpv)
        {
            mpv.API.SetPropertyString("audio-display", "no");
            //_mpv.API.SetPropertyString("d3d11-composition", "yes");

            if (EnableReplayGain)
            {
                mpv.API.SetPropertyString("replaygain", "album");
            }
        }

        protected override void SetupMpvPropertyBeforePlay(MpvPlayer mpv, BaseItemDto media)
        {
            if (EnableReplayGain)
            {
                if (media.NormalizationGain != null && media.NormalizationGain != 0)
                {
                    _mpv?.API.SetPropertyDouble("replaygain-fallback", (double)media.NormalizationGain);
                }
                else
                {
                    _mpv?.API.SetPropertyString("replaygain", "album");
                }
            }
        }

        protected override IEnumerable<(string, string)> GetMediaSources(ObservableCollection<BaseItemDto> list)
        {
            return list.Select(b => (App.JellyfinMusicService.GetMusicStream(b), string.Empty));
        }
        protected override void DoAfterPlay(int index)
        {
            PreCacheNextMusic(index);
        }

        protected override bool UpdateDetailedInfo => false;

        private void PreCacheNextMusic(int index)
        {
            index += 1;
            if (index > CurrentPlayList?.Count - 1)
            {
                return;
            }
            var next = CurrentPlayList?[index];
            //await ImageCacheEx.Instance.PreCacheAsync(App.JellyfinMusicService.GetPrimaryJellyfinImageSmall(next.ImageTags, next.Id));
        }

        protected override void OnPlayerStaterComplete()
        {
            IsPlayBarVisible = true;
        }

        public void ShowPlayBar()
        {
            if (CurrentPlaying != null)
            {
                IsPlayBarVisible = true;
            }
        }

        public void HidePlayBar()
        {
            IsPlayBarVisible = false;
        }

        public void ToggleShowPlayScreen()
        {
            IsPlayScreenVisible = !IsPlayScreenVisible;
        }

        public void ShowPlayScreen()
        {
            IsPlayScreenVisible = true;
        }

        public void HidePlayScreen()
        {
            IsPlayScreenVisible = false;
        }

#if WINDOWS
        protected override void SetupMstcInfo(BaseItemDto? media, SystemMediaTransportControlsDisplayUpdater updater)
        {
            updater.Type = MediaPlaybackType.Music;
            updater.MusicProperties.Artist = media?.GetJellyfinArtists();
            updater.MusicProperties.Title = media?.Name;
        }
#endif
    }
}
