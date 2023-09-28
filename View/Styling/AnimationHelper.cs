using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;


namespace SPTC_APP.View.Styling
{
    public static class AnimationHelper
    {
        public static void FadeOut(this UIElement element, double durationSeconds = 1, Action completedCallback = null)
        {
            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = TimeSpan.FromSeconds(durationSeconds),
            };

            fadeOutAnimation.Completed += (sender, e) =>
            {
                element.Visibility = Visibility.Hidden;
                completedCallback?.Invoke();
            };

            element.BeginAnimation(UIElement.OpacityProperty, fadeOutAnimation);
        }

        public static void FadeIn(this UIElement element, double durationSeconds = 1, Action completedCallback = null)
        {
            element.Visibility = Visibility.Visible;
            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = TimeSpan.FromSeconds(durationSeconds),
            };

            fadeInAnimation.Completed += (sender, e) =>
            {
                completedCallback?.Invoke();
            };

            element.BeginAnimation(UIElement.OpacityProperty, fadeInAnimation);
        }

        public static void AnimateMargin(this FrameworkElement element, Thickness toMargin, double durationSeconds = 1, Action completedCallback = null)
        {
            ThicknessAnimation marginAnimation = new ThicknessAnimation
            {
                From = element.Margin,
                To = toMargin,
                Duration = TimeSpan.FromSeconds(durationSeconds),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            marginAnimation.Completed += (sender, e) =>
            {
                completedCallback?.Invoke();
            };

            element.BeginAnimation(FrameworkElement.MarginProperty, marginAnimation);
        }
    }

}
 