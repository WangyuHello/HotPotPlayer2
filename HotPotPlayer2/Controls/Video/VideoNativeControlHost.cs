using Avalonia;
using Avalonia.Controls;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Controls;
using Avalonia.Platform;
using Avalonia.Threading;
using HotPotPlayer2.Base;
using HotPotPlayer2.Service;
using Mpv.NET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2.Controls.Video
{
    public class VideoNativeControlHost : OpenGlControlBase
    {
        public static VideoPlayerService VideoPlayer => ((AppBase)Application.Current!).VideoPlayer;

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            VideoPlayer?.SetupRenderUpdate = mpv =>
            {
                mpv.UpdateCallback = OpenGLUpdateCallback;
            };
        }

        protected override void OnOpenGlRender(GlInterface gl, int fb)
        {
            var scale = VisualRoot?.RenderScaling ?? 1d;
            var width = Bounds.Width * scale;
            var height = Bounds.Height * scale;

            VideoPlayer.OpenGLRender((int)width, (int)height, fb, flipY: 1);
        }

        private MpvOpenglInitParams_get_proc_addressCallback? _getProcAddress;

        protected override void OnOpenGlInit(GlInterface gl)
        {
            _getProcAddress = (ctx, name) => gl.GetProcAddress(name);
            VideoPlayer.SetupGetProcAddress = mpv => mpv.GetProcAddress = _getProcAddress;
            VideoPlayer.WaitForMpvCreate();
            VideoPlayer.EnsureRenderContextCreated();
        }

        protected override void OnOpenGlDeinit(GlInterface gl)
        {
            VideoPlayer.ReleaseRenderContext();
        }

        private void OpenGLUpdateCallback(IntPtr ctx)
        {
            Dispatcher.UIThread.InvokeAsync(RequestNextFrameRendering, DispatcherPriority.Background);
        }
    }
}
