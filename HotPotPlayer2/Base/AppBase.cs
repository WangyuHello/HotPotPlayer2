using Avalonia;
using Avalonia.Controls;
using HotPotPlayer2.Models;
using HotPotPlayer2.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public abstract string ApplicationVersion { get; }

        public void NavigateBack(bool force = false)
        {
            throw new NotImplementedException();
        }

        public void NavigateTo(string name, object? parameter = null)
        {
            throw new NotImplementedException();
        }

        public void ShowToast(ToastInfo toast)
        {
            throw new NotImplementedException();
        }
    }
}
