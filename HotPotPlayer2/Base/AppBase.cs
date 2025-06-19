using Avalonia;
using Avalonia.Controls;
using HotPotPlayer2.Interop;
using HotPotPlayer2.Models;
using HotPotPlayer2.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if WINDOWS
using Windows.Media;
#endif

namespace HotPotPlayer2.Base
{
    public abstract class AppBase : Application, IServiceLocator
    {
        public AppBase App => this;

        private ConfigBase? config;
        public ConfigBase Config => config ??= new AppConfig();
        public abstract Window MainWindow { get; }

        private JellyfinMusicService? jellyfinMusicService;
        public JellyfinMusicService JellyfinMusicService => jellyfinMusicService ??= new JellyfinMusicService(Config, this);

        private MusicPlayerService? musicPlayer;
        public MusicPlayerService MusicPlayer => musicPlayer ??= new MusicPlayerService(Config, this);

        private VideoPlayerService? videoPlayer;
        public VideoPlayerService VideoPlayer => videoPlayer ??= new VideoPlayerService(Config, this);

#if WINDOWS
        public abstract SystemMediaTransportControls? SMTC { get; set; }
        public abstract void InitSmtc();
        public abstract void SetSmtcStatus(MediaPlaybackStatus status, bool init = false);
        public abstract void SetSmtcPosition(TimeSpan current, TimeSpan? duration);
#endif
        public abstract string ApplicationVersion { get; }
        public abstract string MpvVersion { get; }

        public void NavigateBack(bool force = false)
        {
            throw new NotImplementedException();
        }

        public void NavigateTo(string name, object? parameter = null)
        {
            throw new NotImplementedException();
        }

        public abstract void ShowToast(ToastInfo toast);
        public abstract IntPtr MainWindowHandle { get; }
        public abstract Rect Bounds { get; }

#if WINDOWS
        TaskbarHelper? _taskbar;
        public TaskbarHelper Taskbar => _taskbar ??= new TaskbarHelper(MainWindowHandle);
#endif
    }
}
