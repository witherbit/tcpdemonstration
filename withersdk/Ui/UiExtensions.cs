using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace withersdk.Ui
{
    public static class UiExtensions
    {
        #region Window
        enum AccentState
        {
            ACCENT_DISABLED = 0,
            ACCENT_ENABLE_GRADIENT = 1,
            ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
            ACCENT_ENABLE_BLURBEHIND = 3,
            ACCENT_INVALID_STATE = 4
        }

        [StructLayout(LayoutKind.Sequential)]
        struct AccentPolicy
        {
            public AccentState AccentState;
            public int AccentFlags;
            public int GradientColor;
            public int AnimationId;
        }
        enum WindowCompositionAttribute
        {
            // ...
            WCA_ACCENT_POLICY = 19
            // ...
        }
        [StructLayout(LayoutKind.Sequential)]
        struct WindowCompositionAttributeData
        {
            public WindowCompositionAttribute Attribute;
            public IntPtr Data;
            public int SizeOfData;
        }
        [DllImport("user32.dll")]
        private static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
        internal static void EnableBlur(this Window window)
        {
            var windowHelper = new WindowInteropHelper(window);

            var accent = new AccentPolicy
            {
                AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND,
            };

            int accentStructSize = Marshal.SizeOf(accent);

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }

        #endregion

        #region ContentControl

        public static void OpacityAnimation(this ContentControl instance, double startpoint, double endpoint, double duration)
        {
            DoubleAnimation da = new DoubleAnimation(startpoint, endpoint, TimeSpan.FromSeconds(duration));
            instance.BeginAnimation(ContentControl.OpacityProperty, da);
        }
        public static void OpacityAnimation(this Page page, double startpoint, double endpoint, double duration)
        {
            DoubleAnimation da = new DoubleAnimation(startpoint, endpoint, TimeSpan.FromSeconds(duration));
            page.BeginAnimation(Page.OpacityProperty, da);
        }
        public static void ForegroundFadeTo(this TextBox border, Brush to, double duration)
        {
            ColorAnimation colorChangeAnimation = new ColorAnimation();
            colorChangeAnimation.From = ((SolidColorBrush)border.Foreground).Color;
            colorChangeAnimation.To = ((SolidColorBrush)to).Color;
            colorChangeAnimation.Duration = TimeSpan.FromSeconds(duration);

            PropertyPath colorTargetPath = new PropertyPath($"(TextBox.Foreground).(SolidColorBrush.Color)");
            Storyboard CellBackgroundChangeStory = new Storyboard();
            Storyboard.SetTarget(colorChangeAnimation, border);
            Storyboard.SetTargetProperty(colorChangeAnimation, colorTargetPath);
            CellBackgroundChangeStory.Children.Add(colorChangeAnimation);
            CellBackgroundChangeStory.Begin();
        }

        public static void OpacityAnimation(this Border instance, double startpoint, double endpoint, double duration)
        {
            DoubleAnimation da = new DoubleAnimation(startpoint, endpoint, TimeSpan.FromSeconds(duration));
            instance.BeginAnimation(Border.OpacityProperty, da);
        }

        public static void OpacityAnimation(this TextBlock instance, double startpoint, double endpoint, double duration)
        {
            DoubleAnimation da = new DoubleAnimation(startpoint, endpoint, TimeSpan.FromSeconds(duration));
            instance.BeginAnimation(TextBlock.OpacityProperty, da);
        }

        public static void ForegroundFadeTo(this TextBlock border, Brush to, double duration)
        {
            ColorAnimation colorChangeAnimation = new ColorAnimation();
            colorChangeAnimation.From = ((SolidColorBrush)border.Foreground).Color;
            colorChangeAnimation.To = ((SolidColorBrush)to).Color;
            colorChangeAnimation.Duration = TimeSpan.FromSeconds(duration);

            PropertyPath colorTargetPath = new PropertyPath($"(TextBlock.Foreground).(SolidColorBrush.Color)");
            Storyboard CellBackgroundChangeStory = new Storyboard();
            Storyboard.SetTarget(colorChangeAnimation, border);
            Storyboard.SetTargetProperty(colorChangeAnimation, colorTargetPath);
            CellBackgroundChangeStory.Children.Add(colorChangeAnimation);
            CellBackgroundChangeStory.Begin();
        }
        public static void FontSizeAnimation(this Control grid, double to, double duration)
        {
            grid.BeginAnimation(Control.FontSizeProperty, new DoubleAnimation(to, TimeSpan.FromSeconds(duration)));
        }
        public static void BackgroundFadeTo(this Border border, Brush to, double duration)
        {
            ColorAnimation colorChangeAnimation = new ColorAnimation();
            colorChangeAnimation.From = ((SolidColorBrush)border.Background).Color;
            colorChangeAnimation.To = ((SolidColorBrush)to).Color;
            colorChangeAnimation.Duration = TimeSpan.FromSeconds(duration);

            PropertyPath colorTargetPath = new PropertyPath("(Border.Background).(SolidColorBrush.Color)");
            Storyboard CellBackgroundChangeStory = new Storyboard();
            Storyboard.SetTarget(colorChangeAnimation, border);
            Storyboard.SetTargetProperty(colorChangeAnimation, colorTargetPath);
            CellBackgroundChangeStory.Children.Add(colorChangeAnimation);
            CellBackgroundChangeStory.Begin();
        }
        public static void BorderBrushFadeTo(this Border border, Brush to, double duration)
        {
            ColorAnimation colorChangeAnimation = new ColorAnimation();
            colorChangeAnimation.From = ((SolidColorBrush)border.BorderBrush).Color;
            colorChangeAnimation.To = ((SolidColorBrush)to).Color;
            colorChangeAnimation.Duration = TimeSpan.FromSeconds(duration);

            PropertyPath colorTargetPath = new PropertyPath("(Border.BorderBrush).(SolidColorBrush.Color)");
            Storyboard CellBackgroundChangeStory = new Storyboard();
            Storyboard.SetTarget(colorChangeAnimation, border);
            Storyboard.SetTargetProperty(colorChangeAnimation, colorTargetPath);
            CellBackgroundChangeStory.Children.Add(colorChangeAnimation);
            CellBackgroundChangeStory.Begin();
        }
        public static void BackgroundFadeTo(this Panel panel, Brush to, double duration)
        {
            ColorAnimation colorChangeAnimation = new ColorAnimation();
            colorChangeAnimation.From = ((SolidColorBrush)panel.Background).Color;
            colorChangeAnimation.To = ((SolidColorBrush)to).Color;
            colorChangeAnimation.Duration = TimeSpan.FromSeconds(duration);

            PropertyPath colorTargetPath = new PropertyPath("(Panel.Background).(SolidColorBrush.Color)");
            Storyboard CellBackgroundChangeStory = new Storyboard();
            Storyboard.SetTarget(colorChangeAnimation, panel);
            Storyboard.SetTargetProperty(colorChangeAnimation, colorTargetPath);
            CellBackgroundChangeStory.Children.Add(colorChangeAnimation);
            CellBackgroundChangeStory.Begin();
        }
        public static void BackgroundFadeTo(this Grid grid, Brush to, double duration)
        {
            ColorAnimation colorChangeAnimation = new ColorAnimation();
            colorChangeAnimation.From = ((SolidColorBrush)grid.Background).Color;
            colorChangeAnimation.To = ((SolidColorBrush)to).Color;
            colorChangeAnimation.Duration = TimeSpan.FromSeconds(duration);

            PropertyPath colorTargetPath = new PropertyPath("(Grid.Background).(SolidColorBrush.Color)");
            Storyboard CellBackgroundChangeStory = new Storyboard();
            Storyboard.SetTarget(colorChangeAnimation, grid);
            Storyboard.SetTargetProperty(colorChangeAnimation, colorTargetPath);
            CellBackgroundChangeStory.Children.Add(colorChangeAnimation);
            CellBackgroundChangeStory.Begin();
        }
        public static void BackgroundFadeTo(this StackPanel stackPanel, Brush to, double duration)
        {
            ColorAnimation colorChangeAnimation = new ColorAnimation();
            colorChangeAnimation.From = ((SolidColorBrush)stackPanel.Background).Color;
            colorChangeAnimation.To = ((SolidColorBrush)to).Color;
            colorChangeAnimation.Duration = TimeSpan.FromSeconds(duration);

            PropertyPath colorTargetPath = new PropertyPath("(StackPanel.Background).(SolidColorBrush.Color)");
            Storyboard CellBackgroundChangeStory = new Storyboard();
            Storyboard.SetTarget(colorChangeAnimation, stackPanel);
            Storyboard.SetTargetProperty(colorChangeAnimation, colorTargetPath);
            CellBackgroundChangeStory.Children.Add(colorChangeAnimation);
            CellBackgroundChangeStory.Begin();
        }
        public static void BackgroundFadeTo(this Button button, Brush to, double duration)
        {
            ColorAnimation colorChangeAnimation = new ColorAnimation();
            colorChangeAnimation.From = ((SolidColorBrush)button.Background).Color;
            colorChangeAnimation.To = ((SolidColorBrush)to).Color;
            colorChangeAnimation.Duration = TimeSpan.FromSeconds(duration);

            PropertyPath colorTargetPath = new PropertyPath("(Button.Background).(SolidColorBrush.Color)");
            Storyboard CellBackgroundChangeStory = new Storyboard();
            Storyboard.SetTarget(colorChangeAnimation, button);
            Storyboard.SetTargetProperty(colorChangeAnimation, colorTargetPath);
            CellBackgroundChangeStory.Children.Add(colorChangeAnimation);
            CellBackgroundChangeStory.Begin();
        }
        public static void MarginAnimation(this Control border, Thickness thickness, double duration)
        {
            border.BeginAnimation(Control.MarginProperty, new ThicknessAnimation(thickness, TimeSpan.FromSeconds(0.1)));
        }
        public static void HeightAnimation(this Control grid, double startpoint, double endpoint, double duration)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = startpoint;
            da.To = endpoint;
            da.Duration = TimeSpan.FromSeconds(duration);
            grid.BeginAnimation(Control.HeightProperty, da);
        }
        public static void WidthAnimation(this Control grid, double startpoint, double endpoint, double duration)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = startpoint;
            da.To = endpoint;
            da.Duration = TimeSpan.FromSeconds(duration);
            grid.BeginAnimation(Control.WidthProperty, da);
        }
        public static void MarginAnimation(this FrameworkElement border, Thickness thickness, double duration)
        {
            border.BeginAnimation(FrameworkElement.MarginProperty, new ThicknessAnimation(thickness, TimeSpan.FromSeconds(0.1)));
        }
        public static void CornerRadiusAnimation(this Border border, CornerRadius thickness, double duration)
        {
            border.BeginAnimation(Border.CornerRadiusProperty, new CornerRadiusAnimation(thickness, TimeSpan.FromSeconds(duration)));
        }
        public static void HeightAnimation(this FrameworkElement grid, double startpoint, double endpoint, double duration)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = startpoint;
            da.To = endpoint;
            da.Duration = TimeSpan.FromSeconds(duration);
            grid.BeginAnimation(FrameworkElement.HeightProperty, da);
        }
        public static void WidthAnimation(this FrameworkElement grid, double startpoint, double endpoint, double duration)
        {
            DoubleAnimation da = new DoubleAnimation();
            da.From = startpoint;
            da.To = endpoint;
            da.Duration = TimeSpan.FromSeconds(duration);
            grid.BeginAnimation(FrameworkElement.WidthProperty, da);
        }

        #endregion

        public static SolidColorBrush ConvertToBrush(this string hex)
        {
            if (hex[0] != '#') hex = hex.Insert(0, "#");
            return (SolidColorBrush)new BrushConverter().ConvertFrom(hex);
        }
        public static Color ConvertToColor(this string hex)
        {
            if (hex[0] != '#') hex = hex.Insert(0, "#");
            return (Color)ColorConverter.ConvertFromString(hex);
        }
        public static void Invoke(this ContentControl instanse, Action action)
        {
            instanse.Dispatcher?.BeginInvoke(DispatcherPriority.Background, action);
        }
        public static void Invoke(this Page instanse, Action action)
        {
            instanse.Dispatcher?.BeginInvoke(DispatcherPriority.Background, action);
        }
    }
}
