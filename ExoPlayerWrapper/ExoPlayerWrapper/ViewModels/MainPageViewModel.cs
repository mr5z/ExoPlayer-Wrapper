using System;
using System.Collections.Generic;
using System.Text;
using ExoPlayerWrapper.Controls;
using ExoPlayerWrapper.Helpers;
using ExoPlayerWrapper.Models;
using ExoPlayerWrapper.Services;
using Prism.AppModel;
using Prism.Commands;
using Prism.Navigation;

namespace ExoPlayerWrapper.ViewModels
{
    public class MainPageViewModel : ViewModelBase, IPageLifecycleAware
    {
        private readonly IVideoService videoService;
        private bool isPlaying;

        public MainPageViewModel(INavigationService navigationService, IVideoService videoService) : base(navigationService)
        {
            this.videoService = videoService;

            ForwardCommand = new DelegateCommand(Forward);
            BackwardCommand = new DelegateCommand(Backward);
            PlayPauseCommand = new DelegateCommand(PlayPause);
            SliderValueChangedCommand = new DelegateCommand<HistoryDoubleValue>(SliderValueChanged);

            videoService.VideoPlayerAttached += VideoService_VideoPlayerAttached;
        }

        private void VideoService_VideoPlayerAttached(object sender, EventArgs e)
        {
            videoService.VideoStateChanged += VideoService_VideoStateChanged;
            videoService.CurrentPositionChanged += VideoService_CurrentPositionChanged;
        }

        private void VideoService_CurrentPositionChanged(object sender, PositionChangedEventArgs e)
        {
            if (videoService.Status == VideoState.Playing)
            {
                var duration = videoService.Duration.TotalMilliseconds;
                var position = e.NewPosition.TotalMilliseconds;
                var bufferPosition = e.NewBufferedPosition.TotalMilliseconds;
                var positionPercent = position / duration;
                var bufferedPercent = bufferPosition / duration;
                Debug.Log("Percent: {0}", positionPercent);
                SliderValue = positionPercent;
                BufferedStreamValue = bufferedPercent;
            }
        }

        private void VideoService_VideoStateChanged(object sender, VideoStateChangedEventArgs e)
        {
            switch (e.NewState)
            {
                case VideoState.Configured:
                    videoService.ShowDefaultControls = false;
                    videoService.Load("http://qthttp.apple.com.edgesuite.net/1010qwoeiuryfg/sl.m3u8");
                    break;
                case VideoState.Playing:
                    isPlaying = true;
                    break;
                case VideoState.Paused:
                    isPlaying = false;
                    break;
            }
        }

        private void Forward()
        {
            var step = TimeSpan.FromSeconds(5);
            var currentPosition = videoService.CurrentPosition;
            var leap = (currentPosition + step).TotalMilliseconds;
            var duration = videoService.Duration.TotalMilliseconds;
            SliderValue = Math.Min(1, leap / duration);
            videoService.Forward(step);
        }

        private void Backward()
        {
            var step = TimeSpan.FromSeconds(5);
            var currentPosition = videoService.CurrentPosition;
            var leap = (currentPosition - step).TotalMilliseconds;
            var duration = videoService.Duration.TotalMilliseconds;
            SliderValue = Math.Max(0, leap / duration);
            videoService.Backward(step);
        }

        private void PlayPause()
        {
            if (isPlaying)
            {
                videoService.Pause();
            }
            else
            {
                videoService.Play();
            }
        }

        private void SliderValueChanged(HistoryDoubleValue sliderValue)
        {
            if (!sliderValue.IsValid)
                return;

            var duration = videoService.Duration.TotalMilliseconds;
            var oldPosition = sliderValue.OldValue * duration;
            var newPosition = sliderValue.NewValue * duration;
            var difference = Math.Abs(newPosition - oldPosition);
            var timeDifference = TimeSpan.FromMilliseconds(difference);
            if (timeDifference.TotalSeconds > 1.5)
            {
                // skipped!
                Debug.Log("Skipped!");
                videoService.SeekTo(TimeSpan.FromMilliseconds(newPosition));
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            var mode = parameters.GetNavigationMode();
        }

        public override void Destroy()
        {
            base.Destroy();

            videoService.VideoStateChanged -= VideoService_VideoStateChanged;
            videoService.CurrentPositionChanged -= VideoService_CurrentPositionChanged;
        }

        public void OnAppearing()
        {
        }

        public void OnDisappearing()
        {
        }

        public double SliderValue { get; set; }

        public double BufferedStreamValue { get; set; }

        public DelegateCommand ForwardCommand { get; set; }
        public DelegateCommand BackwardCommand { get; set; }
        public DelegateCommand PlayPauseCommand { get; set; }
        public DelegateCommand<HistoryDoubleValue> SliderValueChangedCommand { get; set; }
    }
}
