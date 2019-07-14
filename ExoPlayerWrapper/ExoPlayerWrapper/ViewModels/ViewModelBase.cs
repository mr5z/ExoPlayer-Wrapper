using PropertyChanged;
using Prism.Navigation;
using Prism.Mvvm;

namespace ExoPlayerWrapper.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        [DoNotNotify]
        protected INavigationService NavigationService { get; private set; }

        public ViewModelBase(INavigationService navigationService)
        {
            NavigationService = navigationService;
        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatingTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }
    }
}
