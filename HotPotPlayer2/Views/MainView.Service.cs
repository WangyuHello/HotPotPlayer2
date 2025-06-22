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
            Toast.Show();
            _timer ??= InitToastTimer();
            _timer.Start();
        }

        public void DismissToast()
        {
            _timer?.Stop();
            Toast.Hide();
            _toastOpened = false;
        }

        private void MusicPlayer_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsPlayBarVisible")
            {
                var visible = (DataContext as MainWindowViewModel)!.MusicPlayer.IsPlayBarVisible;
                if (visible)
                {
                    PlayBar.Show();
                }
                else
                {
                    PlayBar.Hide();
                }
            }
            else if(e.PropertyName == "IsPlayListBarVisible")
            {
                var visible = (DataContext as MainWindowViewModel)!.MusicPlayer.IsPlayListBarVisible;
                if (visible)
                {
                    CurrentPlayListBar.Show();
                }
                else
                {
                    CurrentPlayListBar.Hide();
                }
            }
        }
    }
}
