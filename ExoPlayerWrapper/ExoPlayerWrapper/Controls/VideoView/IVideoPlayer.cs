using System;
using System.Collections.Generic;
using System.Text;

namespace ExoPlayerWrapper.Controls
{
    public interface IVideoPlayer
    {
        bool ShowDefaultControls { get; set; }
        TimeSpan Duration { get; }
        TimeSpan CurrentPosition { get; }
        TimeSpan BufferedPosition { get; }
        VideoState Status { get; }

        void Load(string source);
        void Play();
        void Pause();
        void Stop();
        void SeekTo(TimeSpan position);
        void Forward(TimeSpan step);
        void Backward(TimeSpan step);

        event EventHandler<PositionChangedEventArgs> CurrentPositionChanged;
        event EventHandler<VideoStateChangedEventArgs> VideoStateChanged;
    }

    public class PositionChangedEventArgs : EventArgs
    {
        public PositionChangedEventArgs(TimeSpan newPosition, TimeSpan newBufferedPosition)
        {
            NewPosition = newPosition;
            NewBufferedPosition = newBufferedPosition;
        }

        public TimeSpan NewPosition { get; }
        public TimeSpan NewBufferedPosition { get; }
    }

    public class VideoStateChangedEventArgs : EventArgs
    {
        public VideoStateChangedEventArgs(VideoState newState)
        {
            NewState = newState;
        }
        public VideoState NewState { get; }
    }
}
