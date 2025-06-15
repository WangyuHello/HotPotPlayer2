using Avalonia.Input;
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
        public void PlayModeButtonClick(object sender, RoutedEventArgs e)
        {
            MusicPlayer.TogglePlayMode();
        }
        public void PlayPreviousButtonClick(object sender, RoutedEventArgs e)
        {
            MusicPlayer.PlayPrevious();
        }
        public void PlayNextButtonClick(object sender, RoutedEventArgs e)
        {
            MusicPlayer.PlayNextInPlayList();
        }
        public void PlaySlider_DragStarted(object sender, VectorEventArgs e)
        {
            MusicPlayer.SuppressCurrentTimeTrigger = true;
        }

        public void PlaySlider_DragCompleted(double value)
        {
            MusicPlayer.SuppressCurrentTimeTrigger = false;
            TimeSpan to = GetToTime(value);
            MusicPlayer.PlayTo(to);
        }

        public TimeSpan GetToTime(double value)
        {
            if (MusicPlayer.CurrentPlayingDuration == null)
            {
                return TimeSpan.Zero;
            }
            var percent100 = (int)value;
            var v = percent100 * ((TimeSpan)MusicPlayer.CurrentPlayingDuration).Ticks / 100;
            var to = TimeSpan.FromTicks(v);
            return to;
        }
    }
}
