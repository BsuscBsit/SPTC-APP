using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Xml.Linq;

namespace SPTC_APP.View.Styling
{
    public static class AnimationHelper
    {
        private static readonly EasingFunctionBase DefaultEasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut };

        public static void FadeOut(this UIElement element, double durationSeconds = 1, Action completedCallback = null)
        {
            AnimateOpacity(element, element.Opacity, 0.0, durationSeconds, UIElement.OpacityProperty, completedCallback);
            element.Visibility = Visibility.Hidden;
        }

        public static void FadeIn(this UIElement element, double durationSeconds = 1, Action beginningCallback = null, Action completedCallback = null)
        {
            beginningCallback?.Invoke();
            element.Visibility = Visibility.Visible;
            AnimateOpacity(element, 0.0, 1.0, durationSeconds, UIElement.OpacityProperty, completedCallback);
        }

        public static void FadeOutDropShadow(this UIElement element, double durationSeconds = 1, Action completedCallback = null)
        {
            AnimateOpacity(element, element.Opacity, 0.0, durationSeconds, DropShadowEffect.OpacityProperty, completedCallback);
        }

        public static void FadeInDropShadow(this UIElement element, double durationSeconds = 1, Action beginningCallback = null, Action completedCallback = null)
        {
            beginningCallback?.Invoke();
            AnimateOpacity(element, 0.0, 1.0, durationSeconds, DropShadowEffect.OpacityProperty, completedCallback);
        }

        public static void AnimateMargin(this FrameworkElement element, Thickness toMargin, double durationSeconds = 1, Action completedCallback = null)
        {
            var fromMargin = element.Margin;
            var marginAnimation = new ThicknessAnimation
            {
                From = fromMargin,
                To = toMargin,
                Duration = TimeSpan.FromSeconds(durationSeconds),
                EasingFunction = DefaultEasingFunction
            };

            AnimateProperty(element, FrameworkElement.MarginProperty, marginAnimation, completedCallback);
        }

        public static void AnimateWidth(this FrameworkElement element, double toVal, double durationSeconds = 1, Action completedCallback = null)
        {
            AnimateDoubleProperty(element, FrameworkElement.WidthProperty, element.Width, toVal, durationSeconds, DefaultEasingFunction, completedCallback);
        }

        public static void AnimateHeight(this FrameworkElement element, double toVal, double durationSeconds = 1, Action completedCallback = null)
        {
            AnimateDoubleProperty(element, FrameworkElement.HeightProperty, element.Height, toVal, durationSeconds, DefaultEasingFunction, completedCallback);
        }

        private static void AnimateOpacity(UIElement element, double from, double to, double durationSeconds, DependencyProperty property, Action completedCallback)
        {
            var opacityAnimation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromSeconds(durationSeconds),
            };

            AnimateProperty(element, property, opacityAnimation, completedCallback);
        }

        private static void AnimateProperty(UIElement element, DependencyProperty property, AnimationTimeline animation, Action completedCallback)
        {
            animation.Completed += (sender, e) =>
            {
                completedCallback?.Invoke();
            };

            element.BeginAnimation(property, animation);
        }

        private static void AnimateDoubleProperty(FrameworkElement element, DependencyProperty property, double from, double to, double durationSeconds, EasingFunctionBase easingFunction, Action completedCallback)
        {
            var doubleAnimation = new DoubleAnimation
            {
                From = from,
                To = to,
                Duration = TimeSpan.FromSeconds(durationSeconds),
                EasingFunction = easingFunction
            };

            AnimateProperty(element, property, doubleAnimation, completedCallback);
        }

    }
}

