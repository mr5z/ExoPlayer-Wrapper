using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ExoPlayerWrapper.Services;
using ExoPlayerWrapper.Views;
using Prism;
using Prism.Ioc;
using Prism.Plugin.Popups;
using ExoPlayerWrapper.ViewModels;
using ExoPlayerWrapper.Services.Implementation;

namespace ExoPlayerWrapper
{
    public partial class App
    {
        /* 
         * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
         * This imposes a limitation in which the App class must have a default constructor. 
         * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
         */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            var result = await NavigationService.PushAsync<MainPageViewModel>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterPopupNavigationService();
            RegisterPages(containerRegistry);
            RegisterServices(containerRegistry);
        }

        static void RegisterPages(IContainerRegistry registry)
        {
            registry.RegisterForNavigation<NavigationPage>();
            registry.RegisterForNavigation<MainPage, MainPageViewModel>();
        }

        static void RegisterServices(IContainerRegistry registry)
        {
            registry.RegisterSingleton<IVideoService, VideoService>();
        }
    }
}
