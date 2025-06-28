using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using HotPotPlayer2.Base;
using Jellyfin.Sdk.Generated.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2.Converters
{
    public static class JellyfinImageConverters
    {
        public static FuncValueConverter<BaseItemDto?, Uri?> PrimaryImage => new(
            i =>
            {
                var jellyfin = ((IServiceLocator)Application.Current!).JellyfinMusicService;
                var uri = jellyfin.GetPrimaryJellyfinImage(i?.ImageTags, i?.Id);
                return uri;
            });

        public static FuncValueConverter<BaseItemDto?, Uri?> PrimaryImage32 => new(
            i =>
            {
                var jellyfin = ((IServiceLocator)Application.Current!).JellyfinMusicService;
                var uri = jellyfin.GetPrimaryJellyfinImageWidth(i?.ImageTags, i?.Id, 32);
                return uri;
            });

        public static FuncValueConverter<BaseItemDto?, Uri?> Backdrop = new(i =>
        {
            var jellyfin = ((IServiceLocator)Application.Current!).JellyfinMusicService;
            var uri = jellyfin.GetBackdropJellyfinImage(i?.BackdropImageTags, i?.Id, 860);
            return uri;
        });
    }
}
