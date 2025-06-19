using Avalonia;
using Avalonia.Threading;
using ExCSS;
using HotPotPlayer2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2.Views
{
    public partial class MainView
    {
        public bool ToastVisible
        {
            get { return (bool)GetValue(ToastVisibleProperty)!; }
            set { SetValue(ToastVisibleProperty, value); }
        }

        public static readonly AvaloniaProperty<bool> ToastVisibleProperty =
            AvaloniaProperty.Register<MainView, bool>("ToastVisible");

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
            ToastVisible = true;
            _timer ??= InitToastTimer();
            _timer.Start();
        }

        public void DismissToast()
        {
            _timer?.Stop();
            _toastOpened = false;
            ToastVisible = false;
        }
    }
}
