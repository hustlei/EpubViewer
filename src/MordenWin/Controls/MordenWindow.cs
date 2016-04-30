using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Windows.Shell;

namespace Lei.UI
{
    public class MordenWindow:Window
    {        
        /// <summary>
        /// Identifies the LogoData dependency property.
        /// </summary>
        public static readonly DependencyProperty LogoDataProperty = DependencyProperty.Register("LogoData", typeof(Geometry), typeof(MordenWindow));
        public static readonly DependencyProperty SystemButtonVisibleProperty = DependencyProperty.Register("SystemButtonVisible", typeof(Visibility), typeof(MordenWindow));
        /// <summary>
        /// Gets or sets the path data for the logo displayed in the title area of the window.
        /// </summary>
        public Geometry LogoData
        {
            get { return (Geometry)GetValue(LogoDataProperty); }
            set { SetValue(LogoDataProperty, value); }
        }
        public Visibility SystemButtonVisible
        {
            get { return (Visibility)GetValue(SystemButtonVisibleProperty); }
            set { SetValue(SystemButtonVisibleProperty, value); }
        }
        /// <summary>
        /// Gets or sets the collection of links that appear in the menu in the title area of the window.
        /// </summary>
        public MordenWindow()
        {
            this.DefaultStyleKey = typeof(MordenWindow);
            // associate window commands with this instance
            this.CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            this.CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
            this.Loaded += (sender, args) =>
            {
                if (WindowStyle == WindowStyle.None)
                    (this.Template.FindName("LayoutRoot", this) as
                        Grid).RowDefinitions[0].Height =
                        new GridLength(0);
            };
        }
        public static void SetTheme(Color backColor)
        {
            Application.Current.Resources["Color_ImageOrColor"] = Visibility.Visible;
            Application.Current.Resources["Image_ImageOrColor"] = Visibility.Collapsed;
            Application.Current.Resources["Win_BackGroudColor"] = backColor;
        }

        public static void SetTheme(ImageSource backImage)
        {
            Application.Current.Resources["Color_ImageOrColor"] = Visibility.Collapsed;
            Application.Current.Resources["Image_ImageOrColor"] = Visibility.Visible;
            Application.Current.Resources["Win_BackGroudImage"] = backImage;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }
         

        public RoutedCommand SettingCommand
        {
            get;
            set;
        }
        /// <summary>
        /// Raises the System.Windows.Window.Closed event.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        private void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode == ResizeMode.CanResize || this.ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        private void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = this.ResizeMode != ResizeMode.NoResize;
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private void OnMaximizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        private void OnMinimizeWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        private void OnRestoreWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);
        }
    }

}
