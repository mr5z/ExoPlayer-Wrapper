using ExoPlayerWrapper.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ExoPlayerWrapper.Controls
{
    public class CustomSlider : View
    {
        public static BindableProperty ProgressProperty = BindableHelper.CreateProperty<double>(nameof(Progress));
        public double Progress
        {
            get => (double)GetValue(ProgressProperty);
            set => SetValue(ProgressProperty, value);
        }

        public static BindableProperty TrackMinColorProperty = BindableHelper.CreateProperty<Color>(nameof(TrackMinColor));
        public Color TrackMinColor
        {
            get => (Color)GetValue(TrackMinColorProperty);
            set => SetValue(TrackMinColorProperty, value);
        }

        public static BindableProperty TrackMaxColorProperty = BindableHelper.CreateProperty<Color>(nameof(TrackMaxColor));
        public Color TrackMaxColor
        {
            get => (Color)GetValue(TrackMaxColorProperty);
            set => SetValue(TrackMaxColorProperty, value);
        }

    }
}
