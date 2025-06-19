using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using HotPotPlayer2.Base;
using HotPotPlayer2.Models;
using HotPotPlayer2.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2
{
    public partial class App : AppBase
    {
		private string? applicationVersion;
		public override string ApplicationVersion => applicationVersion ??= GetApplicationVersion();

		private static string GetApplicationVersion()
		{
            var file = Path.Combine(AppContext.BaseDirectory, "HotPotPlayer2.dll");
            var v = GetFileProductVersion(file) ?? "";
            var plus = v.IndexOf('+');
            if (plus > 0)
            {
                return v[..plus];
            }
            return v;
        }

        private string? mpvVersion;
        public override string MpvVersion => mpvVersion ??= GetMpvVersion();

        private static string GetMpvVersion()
        {
#if WINDOWS
            var file = Path.Combine(AppContext.BaseDirectory, "mpv.dll");
#else
            var file = Path.Combine(AppContext.BaseDirectory, "libmpv.so");
#endif
            return GetFileProductVersion(file) ?? "";
        }

        private static string? GetFileProductVersion(string file)
        {
            string? v = null;
            try
            {
                var myFileVersionInfo = FileVersionInfo.GetVersionInfo(file);
                v = myFileVersionInfo.ProductVersion;
            }
            catch (Exception)
            {

            }
            return v;
        }

        private void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
        {
            Config.SaveSettings();
        }

        Window? mainWindow;
        public override Window MainWindow => mainWindow!;

        private nint _mainWindowHandle;
        public override nint MainWindowHandle
        {
            get
            {
                if(_mainWindowHandle == 0)
                {
                    _mainWindowHandle = MainWindow.TryGetPlatformHandle()!.Handle;
                }
                return _mainWindowHandle;
            }
        }

        public override Rect Bounds => throw new System.NotImplementedException();

        public override void ShowToast(ToastInfo toast)
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                (desktop.MainWindow as MainWindow)?.ShowToast(toast);
            }
            else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
            {
                (singleViewPlatform.MainView as MainView)?.ShowToast(toast);
            }
        }
    }
}
