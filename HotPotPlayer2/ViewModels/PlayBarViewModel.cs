using Avalonia.Interactivity;
using HotPotPlayer2.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotPotPlayer2.ViewModels
{
    public class PlayBarViewModel : ViewModelBase
    {
        public void PlayButtonClick(object sender, RoutedEventArgs e)
        {
            MusicPlayer.PlayOrPause();
        }
    }
}
