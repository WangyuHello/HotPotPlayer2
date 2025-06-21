using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using HotPotPlayer2.Base;
using HotPotPlayer2.Models.Collection;
using Jellyfin.Sdk.Generated.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2.ViewModels
{
    public partial class MusicPageViewModel : PageViewModelBase
    {
        public override string? Name => "Music";

        [ObservableProperty]
        public partial bool NoJellyfinVisible { get; set; }

        [ObservableProperty]
        public partial JellyfinItemCollection? JellyfinAlbumList { get; set; }

        [ObservableProperty]
        public partial JellyfinItemCollection JellyfinPlayListList { get; set; }

        [ObservableProperty]
        public partial BaseItemDto? SelectedAlbum { get; set; }

        [ObservableProperty]
        public partial BaseItemDto? SelectedAlbumInfo { get; set; }

        [ObservableProperty]
        public partial List<BaseItemDto>? SelectedAlbumMusicItems { get; set; }

        [ObservableProperty]
        public partial bool AlbumPopupOverlayVisible { get; set; }

        public override async void OnNavigatedTo(object? args)
        {
            if (JellyfinMusicService.IsMusicPageFirstNavigate)
            {
                JellyfinMusicService.IsMusicPageFirstNavigate = false;

                NoJellyfinVisible = !JellyfinMusicService.IsJellfinAvailable();
                JellyfinAlbumList = new JellyfinItemCollection(() => JellyfinMusicService.SelectedMusicLibraryDto, JellyfinMusicService.GetJellyfinAlbumListAsync);
                JellyfinPlayListList = new JellyfinItemCollection(() => JellyfinMusicService.SelectedMusicLibraryDto, JellyfinMusicService.GetJellyfinPlayListsAsync);

                await JellyfinAlbumList.LoadMoreItemsAsync(default);
            }
        }

        public async void AlbumClick(object sender, RoutedEventArgs e)
        {
            var album = (sender as Button)!.Tag as BaseItemDto;
            if (album != null && album != SelectedAlbum)
            {
                SelectedAlbumMusicItems = await JellyfinMusicService.GetAlbumMusicItemsAsync(album);
                SelectedAlbumInfo = await JellyfinMusicService.GetItemInfoAsync(album);
            }
            SelectedAlbum = album;

            AlbumPopupOverlayVisible = true;
        }

        public void PlayAlbumClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button b)
            {
                return;
            }
            if (b.DataContext is BaseItemDto item)
            {
                MusicPlayer.PlayNext(item);
            }
            else if(b.Tag is BaseItemDto item2)
            {
                MusicPlayer.PlayNext(item2);
            }
        }

        public void MusicItemClick(BaseItemDto music, BaseItemDto album)
        {
            MusicPlayer.PlayNext(music, album);
        }

        public async void JellyfinAlbumListLoadMore()
        {
            if (JellyfinAlbumList == null) return;
            if (!JellyfinAlbumList.IsLoading && JellyfinAlbumList.HasMoreItems)
            {
                await JellyfinAlbumList.LoadMoreItemsAsync(default);
            }
        }
        public async void JellyfinPlayListListLoadMore()
        {
            if (JellyfinPlayListList == null) return;
            if (!JellyfinPlayListList.IsLoading && JellyfinPlayListList.HasMoreItems)
            {
                await JellyfinPlayListList.LoadMoreItemsAsync(default);
            }
        }
    }
}
