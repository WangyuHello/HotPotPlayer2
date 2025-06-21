using Jellyfin.Sdk.Generated.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2.Extensions
{
    public static class BaseItemDtoExtensions
    {
        public static string GetJellyfinArtists(this BaseItemDto? item)
        {
            return string.Join(", ", item?.Artists ?? []);
        }

        public static string GetJellyfinDuration(this BaseItemDto? item)
        {
            var runtimeticks = item?.RunTimeTicks ?? 0;
            var t = new TimeOnly(runtimeticks);
            var str = t.ToString("mm\\:ss");
            return str;
        }

        public static string GetJellyfinDurationChinese(this BaseItemDto? item)
        {
            var runtimeticks = item?.RunTimeTicks ?? 0;
            var t = new TimeOnly(runtimeticks);
            string str;
            if (t > new TimeOnly(10, 0))
            {
                str = t.ToString("hh小时mm分钟");
            }
            else if (t > new TimeOnly(1, 0))
            {
                str = t.ToString("h小时mm分钟");
            }
            else
            {
                str = t.ToString("mm分钟");
            }
            return str;
        }
    }
}
