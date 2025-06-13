using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.ComponentModel;
using HotPotPlayer2.Base;
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
        public partial ObservableCollection<BaseItemDto>? JellyfinAlbumList { get; set; }

        public override async void OnNavigatedTo(object? args)
        {
            if (JellyfinMusicService.IsMusicPageFirstNavigate)
            {
                JellyfinMusicService.IsMusicPageFirstNavigate = false;

                var albums = await JellyfinMusicService.GetJellyfinAlbumListAsync(() => JellyfinMusicService.SelectedMusicLibraryDto);
                if (albums != null)
                {
                    JellyfinAlbumList = new(albums);
                }
            }
        }

        public void AlbumGridView_ItemClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
