using Avalonia;
using Avalonia.Animation;
using Avalonia.Media;
using Avalonia.Threading;
using ExCSS;
using HotPotPlayer2.Models;
using HotPotPlayer2.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2.Views
{
    public partial class MainView
    {
        DispatcherTimer? _timer;
        bool _toastOpened = false;

        DispatcherTimer InitToastTimer()
        {
            var t = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            t.Tick += ToastTimerTick;
            return t;
        }

        private void ToastTimerTick(object? sender, object e)
        {
            DismissToast();
        }

        public void ShowToast(ToastInfo toast)
        {
            if (_toastOpened)
            {
                return;
            }
            _toastOpened = true;
            Toast.ToastInfo = toast;
            Toast.RenderTransform = new TranslateTransform(0, 0);
            _timer ??= InitToastTimer();
            _timer.Start();
        }

        public void DismissToast()
        {
            _timer?.Stop();
            _toastOpened = false;
            Toast.RenderTransform = new TranslateTransform(0, -200);
        }

        private void MusicPlayer_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsPlayBarVisible")
            {
                var visible = (DataContext as MainWindowViewModel)!.MusicPlayer.IsPlayBarVisible;
                if (visible)
                {
                    PlayBar.Opacity = 1;
                    PlayBar.RenderTransform = new TranslateTransform(0, 0);
                }
                else
                {
                    PlayBar.Opacity = 0;
                    PlayBar.RenderTransform = new TranslateTransform(0, 80);
                }
            }
        }

        private void OnHidePlayBar()
        {
            (DataContext as MainWindowViewModel)!.MusicPlayer.HidePlayBar();
        }
    }
}
