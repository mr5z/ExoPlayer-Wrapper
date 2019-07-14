using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExoPlayerWrapper
{
    public static class NavigationExtension
    {
        public static Task<INavigationResult> PushAsync<TViewModel>(this INavigationService navigationService)
            where TViewModel : ViewModels.ViewModelBase
        {
            var pageName = ToPageName<TViewModel>();
            return navigationService.NavigateAsync(pageName);
        }

        public static Task<INavigationResult> PushAsync<TViewModel>(this INavigationService navigationService,
            INavigationParameters parameters = null)
            where TViewModel : ViewModels.ViewModelBase
        {
            var pageName = ToPageName<TViewModel>();
            return navigationService.NavigateAsync(pageName, parameters);
        }

        static string ToPageName<TViewModel>() where TViewModel : ViewModels.ViewModelBase
        {
            return typeof(TViewModel).Name.Replace("ViewModel", string.Empty);
        }
    }
}
