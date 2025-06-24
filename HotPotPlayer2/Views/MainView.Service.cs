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

        public bool IsToastVisible
        {
            get { return (bool)GetValue(IsToastVisibleProperty)!; }
            set { SetValue(IsToastVisibleProperty, value); }
        }

        public static readonly AvaloniaProperty<bool> IsToastVisibleProperty =
            AvaloniaProperty.Register<MainView, bool>("IsToastVisible");

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
            IsToastVisible = true;
            _timer ??= InitToastTimer();
            _timer.Start();
        }

        public void DismissToast()
        {
            _timer?.Stop();
            IsToastVisible = false;
            _toastOpened = false;
        }
    }
}
