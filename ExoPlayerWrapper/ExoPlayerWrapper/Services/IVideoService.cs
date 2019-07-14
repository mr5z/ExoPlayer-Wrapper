using ExoPlayerWrapper.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExoPlayerWrapper.Services
{
    public interface IVideoService
    {
        int PlayerId { get; }
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
        void SetVideoPlayer(IVideoPlayer videoPlayer);

        event EventHandler<PositionChangedEventArgs> CurrentPositionChanged;
        event EventHandler<VideoStateChangedEventArgs> VideoStateChanged;
        event EventHandler<EventArgs> VideoPlayerAttached;
    }
}
