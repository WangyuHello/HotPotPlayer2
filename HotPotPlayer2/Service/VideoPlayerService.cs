using Avalonia.Controls.Shapes;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using HotPotPlayer2.Base;
using HotPotPlayer2.Models;
using Jellyfin.Sdk.Generated.Models;
using Mpv.NET.Player;
using Richasy.BiliKernel.Models.Danmaku;
using Richasy.BiliKernel.Models.Media;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#if WINDOWS
using Windows.Graphics.Display;
using Windows.System;
#endif

namespace HotPotPlayer2.Service
{
    public partial class VideoPlayerService(ConfigBase config, AppBase app) : PlayerService(config, app)
    {
        [ObservableProperty]
        public partial VideoPlayVisualState VisualState { get; set; }

        private VideoBasicInfo? _videoBasicInfo;
        public VideoBasicInfo? VideoBasicInfo
        {
            get => _videoBasicInfo;
            set => SetProperty(ref _videoBasicInfo, value);
        }

#if WINDOWS
        //private DisplayInfo? _displayInfo;
#endif

        public override async void PlayNext(BaseItemDto? video)
        {
            if (video == null) { return; }
            if (video.IsFolder!.Value)
            {
                VisualState = VideoPlayVisualState.FullWindow;
                State = PlayerState.Loading;

                var seasons = await App.JellyfinMusicService.GetSeasonsAsync(video);
                var season = seasons?.FirstOrDefault();
                var episodes = await App.JellyfinMusicService.GetEpisodes(season);
                if(episodes != null)
                {
                    CurrentPlayList = [.. episodes];
                }
            }
            else
            {
                CurrentPlayList = [video];
            }
            PlayNext(0);
        }
        protected override void OnPlayNextStateChange(int? index)
        {
            var v = CurrentPlayList![index ?? 0];
            if (v.Etag == "Bilibili")
            {
                VisualState = VideoPlayVisualState.FullHost;
            }
            else
            {
                VisualState = VideoPlayVisualState.FullWindow;
            }
        }

#if WINDOWS
        protected override void BeforePlayerStarter()
        {
            //if (_displayInfo == null && !Config.HasConfig("target-prim"))
            //{
            //    if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22621, 0))
            //    {
            //        var display = DisplayInformationInterop.GetForWindow(App.MainWindowHandle);
            //        var colorInfo = display.GetAdvancedColorInfo();
            //        _displayInfo = new DisplayInfo
            //        {
            //            IsHDR = colorInfo.CurrentAdvancedColorKind == AdvancedColorKind.HighDynamicRange,
            //            MaxLuminanceInNits = colorInfo.MaxLuminanceInNits.ToString()
            //        };
            //    }
            //    else
            //    {
            //        _displayInfo = new DisplayInfo
            //        {
            //            IsHDR = false,
            //        };
            //    }
            //}
        }
#endif

        protected override void SetupMpvInitProperty(MpvPlayer _mpv)
        {
            //_mpv.API.SetPropertyDouble("display-fps-override", 120d);
            //_mpv.API.SetPropertyString("gpu-debug", "yes");
            //_mpv.API.SetPropertyString("vo", "gpu-next");
            _mpv.API.SetPropertyString("vo", "libmpv");
            _mpv.API.SetPropertyString("gpu-context", "auto");
            _mpv.API.SetPropertyString("hwdec", "auto");
            //_mpv.API.SetPropertyString("d3d11-composition", "yes");
            //_mpv.API.SetPropertyString("icc-profile-auto", "yes");
            SetupRenderUpdate?.Invoke(_mpv.API);
            SetupGetProcAddress?.Invoke(_mpv.API);
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                _mpv.API.EnsureRenderContextCreated();
            }).Wait();
            //string peak = "auto";
            //string prim = "bt.709";
            //string trc = "bt.1886";
#if WINDOWS
            //if (_displayInfo != null && !Config.HasConfig("target-prim"))
            //{
            //    if (_displayInfo.IsHDR)
            //    {
            //        peak = _displayInfo.MaxLuminanceInNits!;
            //        prim = "bt.2020";
            //        trc = "pq";
            //    }
            //}
#endif
            //_mpv.API.SetPropertyString("target-peak", Config.GetConfig("target-peak", peak, true));
            //_mpv.API.SetPropertyString("target-prim", Config.GetConfig("target-prim", prim, true));
            //_mpv.API.SetPropertyString("target-trc", Config.GetConfig("target-trc", trc, true));
            //_mpv.API.SetPropertyString("target-colorspace-hint", "yes"); //HDR passthrough
            _mpv.API.SetPropertyString("loop-playlist", "inf");
        }

        public Action<Mpv.NET.API.Mpv>? SetupRenderUpdate;
        public Action<Mpv.NET.API.Mpv>? SetupGetProcAddress;

        protected override void SetupMpvPropertyBeforePlay(MpvPlayer mpv, BaseItemDto media)
        {
            if (media.Etag == "Bilibili")
            {
                //mpv.API.SetPropertyString("ytdl", "no");
                //mpv.API.SetPropertyString("user-agent", BiliBiliService.VideoUserAgent);
                //mpv.API.SetPropertyString("cookies", "yes");
                //var cookieStr = $"Cookie: {App.BiliBiliService.GetCookieString()}";
                //var refererStr = $"Referer:{BiliBiliService.VideoReferer}";
                //mpv.API.SetPropertyString("http-header-fields", $"{cookieStr}\n{refererStr}");

                //if (!_swapChainInited)
                //{
                //    var loc = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
                //    mpv.LoadPlaylist([Path.Combine(loc, "Assets", "LoadingScreen.png")], true);
                //}
            }
        }

        private string? _currentCid = string.Empty;

        protected override IEnumerable<(string video, string audio)> GetMediaSources(ObservableCollection<BaseItemDto> list)
        {
            var lists = list.Select(v =>
            {
                if (v.Etag == "Bilibili")
                {
                    //var ident = new MediaIdentifier(v.PlaylistItemId, v.Name, null);
                    //var page = App.BiliBiliService.GetVideoPageDetailAsync(ident).Result;
                    //string bestVideo = string.Empty;
                    //string bestAudio = string.Empty;
                    //foreach (var part in page.Parts)
                    //{
                    //    _currentCid = part.Identifier.Id;
                    //    var dash = App.BiliBiliService.GetVideoPlayDetailAsync(page.Information.Identifier, Convert.ToInt64(part.Identifier.Id)).Result ?? throw new NullReferenceException("无法找到视频地址");
                    //    var bestFormats = GetBestQuality(dash.Formats);
                    //    var bestVideoDash = GetBestVideo(dash.Videos, bestFormats);
                    //    var bestAudioDash = dash.Audios?.FirstOrDefault();
                    //    bestVideo = GetNonPcdnUrl(bestVideoDash);
                    //    bestAudio = GetNonPcdnUrl(bestAudioDash);
                    //    if (bestVideoDash == null || string.IsNullOrEmpty(bestVideo))
                    //    {
                    //        throw new NullReferenceException("无法找到视频地址");
                    //    }
                    //    _ = App.BiliBiliService.ReportVideoProgressAsync(v.PlaylistItemId, _currentCid, 0);
                    //    break;
                    //}
                    //return (bestVideo, bestAudio);
                    return (string.Empty, string.Empty);
                }
                else if (v.Path != null && v.Id == null)
                {
                    return (v.Path, string.Empty);
                }
                else
                {
                    return (App.JellyfinMusicService.GetVideoStream(v), string.Empty);
                }
            });

            return lists;
        }

        private string GetBestQuality(IList<PlayerFormatInformation> formats)
        {
            var maxPreferQuality = Config.GetConfig("MaxPreferQuality", "8K", true);
            var maxPreferQ = maxPreferQuality switch
            {
                "240" => 6,
                "360" => 16,
                "480" => 32,
                "720" => 64,
                "720P60" => 74,
                "1080" => 80,
                "1080Plus" => 112,
                "1080P60" => 116,
                "4K" => 120,
                "HDR" => 125,
                "DolbyVision" => 126,
                "8K" => 127,
                _ => 999
            };
            //var formats2 = formats.Select(f => (DashEnum)f.Quality).ToList();
            var sels = formats.Where(f => f.Quality <= maxPreferQ).Select(f => f.Quality).ToList();
            sels.Sort();
            return sels[^1].ToString();
        }
        private DashSegmentInformation? GetBestVideo(IList<DashSegmentInformation> list, string format)
        {
            var maxPreferFormat = Config.GetConfig("MaxPreferFormat", "HEVC", true);
            string[] filter = ["av01", "hevc"];
            int filterIndex = maxPreferFormat == "AV1" ? 0 : maxPreferFormat == "HEVC" ? 1 : 2;
            var l = list.Where(d => d.Id == format).Where(d =>
            {
                var found = false;
                for (int i = 0; i < filterIndex; i++)
                {
                    if (d.Codecs.Contains(filter[i]))
                    {
                        found = true;
                        break;
                    }
                }
                return !found;
            });
            var result = l.LastOrDefault();
            return result;
        }

        private static string? GetNonPcdnUrl(DashSegmentInformation? dash)
        {
            if (dash == null) return string.Empty;
            if (!dash.BaseUrl.Contains("mcdn"))
            {
                return dash.BaseUrl;
            }
            else
            {
                var backup = dash.BackupUrls?.Where(s => !s.Contains("mcdn")).FirstOrDefault();
                return backup;
            }
        }

        protected override BaseItemDto SetCustomInfo(BaseItemDto info)
        {
            info.ProgramId = _currentCid;
            return info;
        }

        protected override void CustomReportProgress(BaseItemDto? currentPlaying, TimeSpan CurrentTime, TimeSpan? CurrentTimeDuration)
        {
            if (currentPlaying?.Etag == "Bilibili")
            {
                //_danmakuController?.UpdateTime((uint)CurrentTime.TotalMilliseconds);
                //await App.BiliBiliService.ReportVideoProgressAsync(currentPlaying.PlaylistItemId, currentPlaying.ProgramId, CurrentTime.Seconds);
            }
        }

        protected override void CustomPlayOrPause(bool playing)
        {
            //if (playing)
            //{
            //    _danmakuController?.Resume();
            //}
            //else
            //{
            //    _danmakuController?.Pause();
            //}
        }

        protected override void CustomMediaResumed()
        {
            _videoBasicInfo = GetVideoBasicInfo();
            Dispatcher.UIThread.Post(() =>
            {
                OnPropertyChanged(nameof(VideoBasicInfo));
            }, DispatcherPriority.Background);
        }
        //private readonly List<DanmakuInformation> _cachedDanmakus = [];
        //private AutoResetEvent _danmankuInitFence;
        //private readonly AutoResetEvent _danmankuSwapChainFence = new(false);

        public override void CustomMediaInited(BaseItemDto? current)
        {
            //if (current.Etag == "Bilibili")
            //{
            //    await Task.Run(async () =>
            //    {
            //        _danmankuInitFence ??= new AutoResetEvent(false);
            //        await LoadDanmakuAsync(current);
            //        if (_danmakuController == null)
            //        {
            //            var host = DanmakuInit?.Invoke();
            //            _danmankuSwapChainFence.WaitOne();
            //            UIQueue.TryEnqueue(DispatcherQueuePriority.Low, () =>
            //            {
            //                _danmakuController = new DanmakuFrostMaster(host);
            //                _danmankuInitFence.Set();
            //            });
            //            _danmankuInitFence.WaitOne();
            //            _danmakuController.AddDanmakuList(BilibiliDanmakuParser.GetDanmakuList(_cachedDanmakus, true));
            //            _danmakuController.UpdateTime(0);
            //            //_danmakuController.SetRollingDensity(2);
            //            _danmakuController.SetOpacity(0.8);
            //            _danmakuController.SetRollingAreaRatio(2);
            //            _danmakuController.SetFontFamilyName("ms-appx:///Assets/Font/MiSans-Medium.ttf#MiSans");
            //        }
            //        else
            //        {
            //            _danmakuController.Clear();
            //            _danmakuController.AddDanmakuList(BilibiliDanmakuParser.GetDanmakuList(_cachedDanmakus, true));
            //            _danmakuController.UpdateTime(0);
            //        }
            //    });
            //}
        }

        //private async Task LoadDanmakuAsync(BaseItemDto current)
        //{
        //    var count = Convert.ToInt32(Math.Ceiling(CurrentPlayingDuration.Value.TotalSeconds / 360d));
        //    if (count == 0)
        //    {
        //        count = 1;
        //    }

        //    _cachedDanmakus.Clear();
        //    for (var i = 0; i < count; i++)
        //    {
        //        try
        //        {
        //            var danmakus = await App.BiliBiliService.GetSegmentDanmakusAsync(current.PlaylistItemId, current.ProgramId, i + 1);
        //            _cachedDanmakus.AddRange(danmakus);
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.WriteLine(ex);
        //            break;
        //        }
        //    }
        //}

        protected override void CustomPauseAsStop()
        {
            //_danmakuController?.Clear();
            //_danmakuController?.UpdateTime(0);
        }

        private VideoBasicInfo GetVideoBasicInfo()
        {
            string colormatrix = string.Empty;
            long? width = 0;
            long? height = 0;
            try
            {
                colormatrix = GetPropertyString("video-params/colormatrix") ?? "";
                width = GetPropertyLong("width");
                height = GetPropertyLong("height");
            }
            catch (Exception)
            {

            }

            return new VideoBasicInfo
            {
                ColorMatrix = colormatrix,
                Width = width,
                Height = height
            };
        }

        public void OpenGLRender(int width, int height, int fbo, int format = 0, int flipY = 0)
        {
            _mpv?.API.OpenGLRender(width, height, fbo, format, flipY);
        }

        public void EnsureRenderContextCreated()
        {
            
        }

        public void ReleaseRenderContext()
        {
            _mpv?.API?.ReleaseRenderContext();
        }
    }
}
