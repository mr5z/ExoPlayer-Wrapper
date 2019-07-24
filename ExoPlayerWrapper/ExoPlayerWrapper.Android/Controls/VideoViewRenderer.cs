using System;
using Android.Content;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Ext.Okhttp;
using Com.Google.Android.Exoplayer2.Extractor;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Source.Hls;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.UI;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2.Upstream.Cache;
using Com.Google.Android.Exoplayer2.Util;
using DryIoc;
using ExoPlayerWrapper.Controls;
using ExoPlayerWrapper.Droid.Controls;
using ExoPlayerWrapper.Services;
using Prism.Ioc;
using Square.OkHttp3;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(VideoView), typeof(VideoViewRenderer))]
namespace ExoPlayerWrapper.Droid.Controls
{
    public class VideoViewRenderer : CustomViewRenderer<VideoView, PlayerView>, IVideoPlayer, IPlayerEventListener
    {
        private OkHttpClient httpClient = new OkHttpClient().NewBuilder().Build();
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

        protected override PlayerView OnPrepareControl(ElementChangedEventArgs<VideoView> e)
        {
            return new PlayerView(Context);
        }

        protected override void OnInitialize(ElementChangedEventArgs<VideoView> e)
        {
            base.OnInitialize(e);
            player = ExoPlayerFactory.NewSimpleInstance(Context);
            player.AddListener(this);
            Control.Player = player;
            ReportVideoState(VideoState.Configured);
        }

        protected override void OnCleanUp(ElementChangedEventArgs<VideoView> e)
        {
            base.OnCleanUp(e);
            timerIsRunning = false;
            player.RemoveListener(this);
            player.Release();
        }

        private void StartPositionListenerInterval()
        {
            timerIsRunning = true;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (Status == VideoState.Playing)
                {
                    if (Element != null)
                    {
                        ReportPositionChanged(CurrentPosition, BufferedPosition);
                    }
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

        private void ReportPositionChanged(TimeSpan newPosition, TimeSpan newBufferedPosition)
        {
            CurrentPositionChanged?.Invoke(this, new PositionChangedEventArgs(newPosition, newBufferedPosition));
            Element.ReportCurrentPositionChanged(newPosition, newBufferedPosition);
        }

        public bool ShowDefaultControls
        {
            get => Control.UseController;
            set => Control.UseController = value;
        }

        public TimeSpan Duration => TimeSpan.FromMilliseconds(player.Duration);

        public TimeSpan CurrentPosition => TimeSpan.FromMilliseconds(player.CurrentPosition);

        public TimeSpan BufferedPosition => TimeSpan.FromMilliseconds(player.BufferedPosition);

        public VideoState Status { get; private set; }

        public void Load(string source)
        {
            var mediaSource = BuildCachedSource(source);
            player.Prepare(mediaSource);
            StopPositionListenerInterval();
            StartPositionListenerInterval();
        }

        //private SimpleExoPlayer BuildPlayer()
        //{
        //    var DEFAULT_BUFFER_FOR_PLAYBACK_AFTER_REBUFFER_MS = 1000 * 60 * 5;
        //    var allocator = new DefaultAllocator(false, DEFAULT_BUFFER_FOR_PLAYBACK_AFTER_REBUFFER_MS);
        //    var loadControl = new DefaultLoadControl(allocator, 30_000, 45_000, 1_500, DEFAULT_BUFFER_FOR_PLAYBACK_AFTER_REBUFFER_MS, 30_000, true);
        //    return ExoPlayerFactory.NewSimpleInstance(Context, loadControl);
        //}

        private IMediaSource BuildHlsSource(string source)
        {
            var userAgent = Util.GetUserAgent(Context, "ExoPlayerWrapper");
            var factory = new DefaultDataSourceFactory(Context, userAgent);
            var uri = Android.Net.Uri.Parse(source);
            var hls = new HlsMediaSource.Factory(factory);
            return hls.CreateMediaSource(uri);
        }

        private IMediaSource BuildCachedSource(string source)
        {
            //var userAgent = Util.GetUserAgent(Context, "ExoPlayerWrapper");
            //var factory = new DefaultDataSourceFactory(Context, userAgent);
            //var httpDataSource = new OkHttpDataSource(httpClient, userAgent, null);
            //var cacheDirectory = new Java.IO.File(Context.CacheDir, "media");
            //var evictor = new LeastRecentlyUsedCacheEvictor(100 * 1024 * 1024);
            //var cache = new SimpleCache(cacheDirectory, evictor);
            //var dataSink = new CacheDataSink(cache, 10 * 1024 * 1024);
            //var fileDataSource = new FileDataSource();
            //var flags = CacheDataSource.FlagBlockOnCache | CacheDataSource.FlagIgnoreCacheOnError;
            //var cacheSource = new CacheDataSource(cache, 
            //    httpDataSource,
            //    fileDataSource,
            //    dataSink,
            //    flags, 
            //    null);
            //var cacheFactory = new CacheDataSourceFactory(cache, factory);
            var cacheFactory = new XCacheDataSourceFactory(Context.ApplicationContext, 100 * 1024 * 1024, 5 * 1024 * 1024);
            var uri = Android.Net.Uri.Parse(source);
            var ex = new ExtractorMediaSource(uri, cacheFactory, new DefaultExtractorsFactory(), null, null);
            var a = new DefaultHlsExtractorFactory().CreateExtractor()
            //var hls = new HlsMediaSource.Factory(cacheFactory);
            return ex;
        }

        public void Pause()
        {
            player.PlayWhenReady = false;
        }

        public void Play()
        {
            player.PlayWhenReady = true;
        }

        public void Stop()
        {
            player.Stop();
        }

        public void SeekTo(TimeSpan position)
        {
            var newPosition = Math.Clamp(position.TotalMilliseconds, 0, Duration.TotalMilliseconds);
            ReportPositionChanged(TimeSpan.FromMilliseconds(newPosition), BufferedPosition);
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
                    ReportVideoState(VideoState.Stopped);
                    break;
                case Player.StateIdle:
                    ReportVideoState(VideoState.Idle);
                    break;
                case Player.StateReady:
                    ReportVideoState(playWhenReady ? VideoState.Playing : VideoState.Paused);
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

        internal class XCacheDataSourceFactory : Java.Lang.Object, IDataSourceFactory
        {
            private readonly Context context;
            private readonly DefaultDataSourceFactory defaultDatasourceFactory;
            private readonly long maxFileSize, maxCacheSize;

            public XCacheDataSourceFactory(Context context, long maxCacheSize, long maxFileSize)
            {
                this.context = context;
                this.maxCacheSize = maxCacheSize;
                this.maxFileSize = maxFileSize;
                var userAgent = Util.GetUserAgent(context, "ExoPlayerShit");
                var bandwidthMeter = new DefaultBandwidthMeter();
                defaultDatasourceFactory = new DefaultDataSourceFactory(context,
                        bandwidthMeter,
                        new DefaultHttpDataSourceFactory(userAgent, bandwidthMeter));
            }

            public IDataSource CreateDataSource()
            {
                var cacheDirectory = new Java.IO.File(context.CacheDir, "wtf");
                if (cacheDirectory.Exists())
                    cacheDirectory.Delete();
                var evictor = new LeastRecentlyUsedCacheEvictor(maxCacheSize);
                var cache = new SimpleCache(cacheDirectory, evictor);
                return new CacheDataSource(cache, defaultDatasourceFactory.CreateDataSource(),
                        new FileDataSource(), new CacheDataSink(cache, maxFileSize),
                        CacheDataSource.FlagBlockOnCache | CacheDataSource.FlagIgnoreCacheOnError, null);
            }
        }
}

}