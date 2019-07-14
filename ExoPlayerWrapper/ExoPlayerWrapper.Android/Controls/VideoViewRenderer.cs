﻿using System;
using Android.Content;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Source.Hls;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.UI;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Util;
using DryIoc;
using ExoPlayerWrapper.Controls;
using ExoPlayerWrapper.Services;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
public class VideoViewRenderer : ViewRenderer<VideoView, PlayerView>, IVideoPlayer, IPlayerEventListener
{
    private SimpleExoPlayer player;
    private bool timerIsRunning;

    public event EventHandler<PositionChangedEventArgs> CurrentPositionChanged;
    public event EventHandler<VideoStateChangedEventArgs> VideoStateChanged;

    private IContainerProvider Container => Prism.PrismApplicationBase.Current.Container;

    public VideoViewRenderer(Context context) : base(context)
    {
        var videoService = Container.Resolve<IVideoService>();
        videoService.SetVideoPlayer(this);
    }

    protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
    {
        base.OnElementChanged(e);

        if (e.NewElement != null)
        {
            if (Control == null)
            {
                var playerView = new PlayerView(Context);
                player = ExoPlayerFactory.NewSimpleInstance(Context);
                player.AddListener(this);
                playerView.Player = player;
                SetNativeControl(playerView);
                ReportVideoState(VideoState.Configured);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            player.Release();
        }
        base.Dispose(disposing);
    }

    private void StartPositionListenerInterval()
    {
        timerIsRunning = true;
        Xamarin.Forms.Device.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            if (Status != VideoState.NotReady && Status != VideoState.Configured)
            {
                ReportPositionChanged(CurrentPosition);
            }
            return timerIsRunning;
        });
    }

    private void StopPositionListenerInterval()
    {
        timerIsRunning = false;
    }

    private void ReportVideoState(VideoState newState)
    {
        Status = newState;
        VideoStateChanged?.Invoke(this, new VideoStateChangedEventArgs(newState));
        Element.ReportVideoStateChanged(newState);
    }

    private void ReportPositionChanged(TimeSpan newPosition)
    {
        CurrentPositionChanged?.Invoke(this, new PositionChangedEventArgs(newPosition));
        Element.ReportCurrentPositionChanged(newPosition);
    }

    public TimeSpan Duration => TimeSpan.FromMilliseconds(player.Duration);

    public TimeSpan CurrentPosition => TimeSpan.FromMilliseconds(player.CurrentPosition);

    public VideoState Status { get; private set; }

    public void Load(string source)
    {
        var userAgent = Util.GetUserAgent(Context, "ExoPlayerWrapper");
        var factory = new DefaultDataSourceFactory(Context, userAgent);
        var mediaSource = new HlsMediaSource.Factory(factory).CreateMediaSource(Android.Net.Uri.Parse(source));
        player.Prepare(mediaSource);
        StopPositionListenerInterval();
        StartPositionListenerInterval();
    }

    public void Pause()
    {
        player.PlayWhenReady = false;
        ReportVideoState(VideoState.Paused);
    }

    public void Play()
    {
        player.PlayWhenReady = true;
        ReportVideoState(VideoState.Playing);
    }

    public void SeekTo(TimeSpan position)
    {
        var newPosition = Math.Clamp(position.TotalMilliseconds, 0, Duration.TotalMilliseconds);
        player.SeekTo((long)newPosition);
    }

    public void Forward(TimeSpan step)
    {
        var totalSteps = CurrentPosition + step;
        SeekTo(totalSteps);
    }

    public void Backward(TimeSpan step)
    {
        var totalSteps = CurrentPosition - step;
        SeekTo(totalSteps);
    }

    public void OnLoadingChanged(bool isLoading)
    {

    }

    public void OnPlaybackParametersChanged(PlaybackParameters playbackParameters)
    {

    }

    public void OnPlayerError(ExoPlaybackException error)
    {
    }

    public void OnPlayerStateChanged(bool playWhenReady, int playbackState)
    {
        switch (playbackState)
        {
            case Player.StateBuffering:
                ReportVideoState(VideoState.Buffering);
                break;
            case Player.StateEnded:
                break;
            case Player.StateIdle:
                ReportVideoState(VideoState.Idle);
                break;
            case Player.StateReady:
                ReportVideoState(VideoState.Ready);
                break;
        }
    }

    public void OnPositionDiscontinuity(int reason)
    {

    }

    public void OnRepeatModeChanged(int repeatMode)
    {

    }

    public void OnSeekProcessed()
    {

    }

    public void OnShuffleModeEnabledChanged(bool shuffleModeEnabled)
    {

    }

    public void OnTimelineChanged(Timeline timeline, Java.Lang.Object manifest, int reason)
    {

    }

    public void OnTracksChanged(TrackGroupArray trackGroups, TrackSelectionArray trackSelections)
    {

    }
}