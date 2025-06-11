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
    public interface IServiceLocator
    {
        public AppBase App { get; }
        public ConfigBase Config { get; }
        public Window MainWindow { get; }

        public JellyfinMusicService JellyfinMusicService { get; }
        public MusicPlayerService MusicPlayer { get; }

        public void ShowToast(ToastInfo toast);
        public void NavigateTo(string name, object? parameter = default);
        public void NavigateBack(bool force = false);
    }
}
