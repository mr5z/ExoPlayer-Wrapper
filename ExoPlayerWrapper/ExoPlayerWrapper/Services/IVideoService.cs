using ExoPlayerWrapper.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExoPlayerWrapper.Services
{
    public interface IVideoService
    {
        int PlayerId { get; }

        void Load(string source);
        void Play();
        void Pause();
        void SeekTo(TimeSpan position);
        void Forward(TimeSpan step);
        void Backward(TimeSpan step);
        void SetVideoPlayer(IVideoPlayer videoPlayer);

        TimeSpan Duration { get; }
        TimeSpan CurrentPosition { get; }
        VideoState Status { get; }

        event EventHandler<PositionChangedEventArgs> CurrentPositionChanged;
        event EventHandler<VideoStateChangedEventArgs> VideoStateChanged;
        event EventHandler<EventArgs> VideoPlayerAttached;
    }
}
