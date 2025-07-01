using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2.Models
{
    public record DisplayInfo
    {
        public bool IsHDR { get; set; }
        public string? MaxLuminanceInNits { get; set; }
    }
}
