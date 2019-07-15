using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ExoPlayerWrapper.Helpers
{
    public static class BindableHelper
    {
        public static BindableProperty CreateProperty<T>(string propertyName, T defaultValue = default, BindingMode mode = BindingMode.TwoWay)
        {
            return BindableProperty.Create(propertyName, typeof(T), typeof(BindableObject), defaultValue, mode);
        }
    }
}
