using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ExoPlayerWrapper.Controls;
using Xamarin.Forms.Platform.Android;

namespace ExoPlayerWrapper.Droid.Controls
{
    class CustomSliderRenderer : CustomViewRenderer<CustomSlider, View>
    {
        public CustomSliderRenderer(Context context) : base(context)
        {
        }

        protected override View OnPrepareControl(ElementChangedEventArgs<CustomSlider> e)
        {
            base.OnPrepareControl(e);
            return new View(Context);
        }

        protected override void OnInitialize(ElementChangedEventArgs<CustomSlider> e)
        {
            base.OnInitialize(e);
            Control.Touch += Control_Touch;
        }

        protected override void OnCleanUp(ElementChangedEventArgs<CustomSlider> e)
        {
            base.OnCleanUp(e);
            Control.Touch -= Control_Touch;
        }

        private void Control_Touch(object sender, TouchEventArgs e)
        {
            switch (e.Event.Action)
            {
                case MotionEventActions.Down:
                    // Click!
                    break;
            }
        }
    }
}