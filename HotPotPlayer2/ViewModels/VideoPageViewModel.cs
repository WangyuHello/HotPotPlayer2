using Avalonia;
using Avalonia.Controls;
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
    public partial class VideoPageViewModel : PageViewModelBase
    {
        public override string? Name => "Video";

        [ObservableProperty]
        public partial int SelectedPivotIndex { get; set; }

        [ObservableProperty]
        public partial bool NoJellyfinVisible { get; set; }

        [ObservableProperty]
        public partial BaseItemDto? SelectedSeries { get; set; }

        [ObservableProperty]
        public partial ObservableCollection<TabItem>? VideoGridViews { get; set; }

        private List<BaseItemDto>? videoViews;
        private List<JellyfinItemCollection>? videoLists;

        public override async void OnNavigatedTo(object? args)
        {
            if (JellyfinMusicService.IsVideoPageFirstNavigate)
            {
                JellyfinMusicService.IsVideoPageFirstNavigate = false;

                videoViews = await JellyfinMusicService.GetVideoViews();
                if (videoViews == null)
                {
                    NoJellyfinVisible = true;
                    return;
                }
                NoJellyfinVisible = false;

                videoLists = [.. videoViews.Select(v => new JellyfinItemCollection(() => v, JellyfinMusicService.GetJellyfinVideoListAsync))];

                int i = 0;
                VideoGridViews = [];
                foreach (var videoView in videoViews)
                {
                    var itemsRepeater = new ItemsRepeater
                    {
                        ItemsSource = videoLists[i]
                    };
                    itemsRepeater.Classes.Add("SeriesCardGridView");

                    var scrollViewer = new ScrollViewer
                    {
                        Margin = new Thickness(-8, 0, -10, 0),
                        Content = itemsRepeater,
                        Tag = i
                    };
                    scrollViewer.ScrollChanged += OnScrollChanged;

                    VideoGridViews.Add(new TabItem
                    {
                        Header = videoView.Name,
                        Content = scrollViewer,
                    });

                    if(i == 0)
                    {
                        await videoLists[i].LoadMoreItemsAsync(default);
                    }

                    i++;
                }
            }
        }

        async partial void OnSelectedPivotIndexChanged(int value)
        {
            if (value == -1 || value == 0) return;
            await videoLists![value].LoadMoreItemsAsync(default);
        }

        private async void OnScrollChanged(object? sender, ScrollChangedEventArgs? e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                if (scrollViewer.Offset.Y >= (scrollViewer.ScrollBarMaximum.Y - 50) && scrollViewer.ScrollBarMaximum.Y > 0)
                {
                    var i = (int)scrollViewer.Tag!;
                    await videoLists![i].LoadMoreItemsAsync(default);
                }
            }
        }
    }
}
