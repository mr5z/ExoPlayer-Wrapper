using Android.Content;
using Xamarin.Forms.Platform.Android;

namespace ExoPlayerWrapper.Droid.Controls
{
    public abstract class CustomViewRenderer<TView, TNativeView> : ViewRenderer<TView, TNativeView>
        where TView : Xamarin.Forms.View
        where TNativeView : Android.Views.View
    {
        public CustomViewRenderer(Context context) : base(context)
        {
        }

        protected override sealed void OnElementChanged(ElementChangedEventArgs<TView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var nativeView = OnPrepareControl(e);
                SetNativeControl(nativeView);
            }

            if (e.NewElement != null)
            {
                OnInitialize(e);
            }

            if (e.OldElement != null)
            {
                OnCleanUp(e);
            }
        }

        protected virtual void OnInitialize(ElementChangedEventArgs<TView> e)
        {

        }

        protected virtual void OnCleanUp(ElementChangedEventArgs<TView> e)
        {

        }

        protected abstract TNativeView OnPrepareControl(ElementChangedEventArgs<TView> e);
    }
}