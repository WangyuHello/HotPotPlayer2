using CommunityToolkit.Mvvm.ComponentModel;
using HotPotPlayer2.Base;
using HotPotPlayer2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2.Service
{
    public partial class VideoPlayerService(ConfigBase config, AppBase app) : PlayerService(config, app)
    {
        [ObservableProperty]
        public partial VideoPlayVisualState VisualState { get; set; }
    }
}
