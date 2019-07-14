using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ExoPlayerWrapper.Controls;

namespace ExoPlayerWrapper.Services.Implementation
{
    public class VideoService : IVideoService
    {
        private IVideoPlayer videoPlayer;

        static int idGenerator = 0;

        public VideoService()
        {
            PlayerId = ++idGenerator;
        }

        public void SetVideoPlayer(IVideoPlayer videoPlayer)
        {
            this.videoPlayer = videoPlayer;
            VideoPlayerAttached?.Invoke(this, EventArgs.Empty);
        }

        public int PlayerId { get; private set; }

        public bool ShowDefaultControls
        {
            get => videoPlayer.ShowDefaultControls;
            set => videoPlayer.ShowDefaultControls = value;
        }

        public TimeSpan Duration => videoPlayer.Duration;

        public TimeSpan CurrentPosition => videoPlayer.CurrentPosition;

        public TimeSpan BufferedPosition => videoPlayer.BufferedPosition;

        public VideoState Status => videoPlayer.Status;

        public event EventHandler<EventArgs> VideoPlayerAttached;

        public event EventHandler<PositionChangedEventArgs> CurrentPositionChanged
        {
            add
            {
                EnsureVideoPlayerNotNull();
                videoPlayer.CurrentPositionChanged += value;
            }
            remove
            {
                EnsureVideoPlayerNotNull();
                videoPlayer.CurrentPositionChanged -= value;
            }
        }
        public event EventHandler<VideoStateChangedEventArgs> VideoStateChanged
        {
            add
            {
                EnsureVideoPlayerNotNull();
                videoPlayer.VideoStateChanged += value;
            }
            remove
            {
                EnsureVideoPlayerNotNull();
                videoPlayer.VideoStateChanged -= value;
            }
        }

        public void Backward(TimeSpan step)
        {
            EnsureVideoPlayerNotNull();
            videoPlayer.Backward(step);
        }

        public void Forward(TimeSpan step)
        {
            EnsureVideoPlayerNotNull();
            videoPlayer.Forward(step);
        }

        public void Load(string source)
        {
            EnsureVideoPlayerNotNull();
            videoPlayer.Load(source);
        }

        public void Pause()
        {
            EnsureVideoPlayerNotNull();
            videoPlayer.Pause();
        }

        public void Play()
        {
            EnsureVideoPlayerNotNull();
            videoPlayer.Play();
        }

        public void Stop()
        {
            EnsureVideoPlayerNotNull();
            videoPlayer.Stop();
        }

        public void SeekTo(TimeSpan position)
        {
            EnsureVideoPlayerNotNull();
            videoPlayer.SeekTo(position);
        }

        private void EnsureVideoPlayerNotNull()
        {
            if (videoPlayer == null)
                throw new ArgumentNullException("Listen for other events after VideoPlayerAttached is invoked");
        }
    }
}
