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
        public static string GetJellyfinArtists(this BaseItemDto item)
        {
            return string.Join(", ", item.Artists ?? []);
        }
    }
}
