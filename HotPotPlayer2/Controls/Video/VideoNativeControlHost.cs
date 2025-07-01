using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2.Controls.Video
{
    public class VideoNativeControlHost : NativeControlHost
    {
        public nint Hwnd { get; set; }
        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            var handle = base.CreateNativeControlCore(parent);
            Hwnd = handle.Handle;
            return handle;
        }
    }
}
