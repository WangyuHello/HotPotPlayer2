using CommunityToolkit.Mvvm.ComponentModel;
using HotPotPlayer2.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2.ViewModels
{
    public partial class BilibiliPageViewModel : PageViewModelBase
    {
        public override string? Name => "Bilibili";

        [ObservableProperty]
        public partial int SelectedSubPage { get; set; }

        public void RefreshClick()
        {

        }
    }
}
