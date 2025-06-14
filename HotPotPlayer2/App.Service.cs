using Avalonia;
using Avalonia.Controls;
using HotPotPlayer2.Base;
using System;
using System.Collections.Generic;
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
			var version = "1.0.0.0";
			return version;
		}

        private void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
        {
            Config.SaveSettings();
        }

        public override Window MainWindow => MainWindow;

        public override nint MainWindowHandle => throw new System.NotImplementedException();

        public override Rect Bounds => throw new System.NotImplementedException();
    }
}
