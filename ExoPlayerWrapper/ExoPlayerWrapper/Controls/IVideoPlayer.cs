using System;
using System.Collections.Generic;
using System.Text;

namespace ExoPlayerWrapper.Controls
{
    public interface IVideoPlayer
    {
        void Load(string source);
        void Play();
        void Pause();
        void SeekTo(TimeSpan position);
        void Forward(TimeSpan step);
        void Backward(TimeSpan step);

        TimeSpan Duration { get; }
        TimeSpan CurrentPosition { get; }
        VideoState Status { get; }

        event EventHandler<PositionChangedEventArgs> CurrentPositionChanged;
        event EventHandler<VideoStateChangedEventArgs> VideoStateChanged;
    }

    public class PositionChangedEventArgs : EventArgs
    {
        public PositionChangedEventArgs(TimeSpan newPosition)
        {
            NewPosition = newPosition;
        }

        public TimeSpan NewPosition { get; }
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
