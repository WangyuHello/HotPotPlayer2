using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using HotPotPlayer2.Models;
using HotPotPlayer2.Service;

namespace HotPotPlayer2.Base
{
    public class ViewModelBase : ObservableObject, IServiceLocator
    {
        public AppBase App => (AppBase)Application.Current!;

        public ConfigBase Config => App.Config;

        public TopLevel Top => App.Top;

        public JellyfinMusicService JellyfinMusicService => App.JellyfinMusicService;

        public MusicPlayerService MusicPlayer => App.MusicPlayer;
        public VideoPlayerService VideoPlayer => App.VideoPlayer;

        public void NavigateBack(bool force = false)
        {
            App.NavigateBack(force);
        }

        public void NavigateTo(string name, object? parameter = null)
        {
            App.NavigateTo(name, parameter);
        }

        public void ShowToast(ToastInfo toast)
        {
            App.ShowToast(toast);
        }
    }
}
