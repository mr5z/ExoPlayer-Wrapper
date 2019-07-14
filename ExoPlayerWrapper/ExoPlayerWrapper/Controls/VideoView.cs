using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ExoPlayerWrapper.Controls
{
    public class VideoView : View
    {
        event EventHandler<PositionChangedEventArgs> CurrentPositionChanged;
        event EventHandler<VideoStateChangedEventArgs> VideoStateChanged;

        public void ReportCurrentPositionChanged(TimeSpan newPosition, TimeSpan newBufferedPosition)
        {
            CurrentPositionChanged?.Invoke(this, new PositionChangedEventArgs(newPosition, newBufferedPosition));
        }

        public void ReportVideoStateChanged(VideoState newState)
        {
            VideoStateChanged?.Invoke(this, new VideoStateChangedEventArgs(newState));
        }
    }
}
