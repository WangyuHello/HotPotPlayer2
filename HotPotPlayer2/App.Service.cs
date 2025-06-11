using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2
{
    public partial class App
    {
		private string? applicationVersion;
		public override string ApplicationVersion => applicationVersion ??= GetApplicationVersion();

		private static string GetApplicationVersion()
		{
			var version = "1.0.0.0";
			return version;
		}
	}
}
