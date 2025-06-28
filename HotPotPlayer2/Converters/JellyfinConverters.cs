using Avalonia.Data.Converters;
using HotPotPlayer2.Extensions;
using HotPotPlayer2.Models.Jellyfin;
using Jellyfin.Sdk.Generated.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2.Converters
{
    public static class JellyfinConverters
    {
        public static FuncValueConverter<BaseItemDto, string> GetJellyfinArtists = new(b =>
        {
            return b.GetJellyfinArtists();
        });

        public static FuncValueConverter<BaseItemDto, string> GetJellyfinDuration = new(b =>
        {
            return b.GetJellyfinDuration();
        });

        public static FuncValueConverter<CustomChapterInfo?, string> GetJellyfinDuration2 = new(b =>
        {
            var t = new TimeOnly(b?.StartPositionTicks ?? 0);
            var str = t.ToString("mm\\:ss");
            return str;
        });

        public static FuncValueConverter<BaseItemDto, string> GetJellyfinDurationChinese = new(b =>
        {
            return b.GetJellyfinDurationChinese();
        });
    }
}
