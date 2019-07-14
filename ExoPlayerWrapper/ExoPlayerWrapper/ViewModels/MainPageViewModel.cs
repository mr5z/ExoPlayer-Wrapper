using System;
using System.Collections.Generic;
using System.Text;
using ExoPlayerWrapper.Controls;
using ExoPlayerWrapper.Helpers;
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

            videoService.VideoPlayerAttached += VideoService_VideoPlayerAttached;
        }

        private void VideoService_VideoPlayerAttached(object sender, EventArgs e)
        {
            videoService.VideoStateChanged += VideoService_VideoStateChanged;
            videoService.CurrentPositionChanged += VideoService_CurrentPositionChanged;
        }

        private void VideoService_CurrentPositionChanged(object sender, PositionChangedEventArgs e)
        {
            Debug.Log("position: {0}", e.NewPosition);
        }

        private void VideoService_VideoStateChanged(object sender, VideoStateChangedEventArgs e)
        {
            switch (e.NewState)
            {
                case VideoState.Configured:
                    videoService.Load("https://bitdash-a.akamaihd.net/content/MI201109210084_1/m3u8s/f08e80da-bf1d-4e3d-8899-f0f6155f6efa.m3u8");
                    videoService.Play();
                    isPlaying = true;
                    break;
            }
        }

        private void Forward()
        {
            videoService.Forward(TimeSpan.FromSeconds(5));
        }

        private void Backward()
        {
            videoService.Backward(TimeSpan.FromSeconds(5));
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
            isPlaying = !isPlaying;
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

        public DelegateCommand ForwardCommand { get; set; }
        public DelegateCommand BackwardCommand { get; set; }
        public DelegateCommand PlayPauseCommand { get; set; }
    }
}
