using Jellyfin.Sdk.Generated.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2.Models.Jellyfin
{
    public class CustomChapterInfo : ChapterInfo
    {
        public int Index { get; set; }
        public Guid ParentId { get; set; }
    }
}
