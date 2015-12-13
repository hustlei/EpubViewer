using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;  

namespace EpubViewer
{
    /// <summary>
    /// LoadingWait.xaml 的交互逻辑
    /// </summary>
    public partial class Waiting : StackPanel
    {
        #region Data   
        private readonly DispatcherTimer animationTimer;  
        #endregion  
 
        #region Constructor   
        public Waiting()  
        {
            InitializeComponent();
  
            animationTimer = new DispatcherTimer(DispatcherPriority.ContextIdle, Dispatcher);  
            animationTimer.Interval = TimeSpan.FromSeconds(0.15);  
        }  
        #endregion  
 
        #region Private Methods
        private void Start()  
        {  
            animationTimer.Tick += HandleAnimationTick;
            animationTimer.Start();
        }  
  
        private void Stop()  
        {  
            animationTimer.Stop();  
            animationTimer.Tick -= HandleAnimationTick;  
        }  
  
        private void HandleAnimationTick(object sender, EventArgs e)
        {
            //SpinnerRotate.Angle = (SpinnerRotate.Angle + 36) % 360; 
            C0.Opacity -= 0.1; if (C0.Opacity < 0.2) C0.Opacity = 1;
            C1.Opacity -= 0.1; if (C1.Opacity < 0.2) C1.Opacity = 1;
            C2.Opacity -= 0.1; if (C2.Opacity < 0.2) C2.Opacity = 1;
            C3.Opacity -= 0.1; if (C3.Opacity < 0.2) C3.Opacity = 1;
            C4.Opacity -= 0.1; if (C4.Opacity < 0.2) C4.Opacity = 1;
            C5.Opacity -= 0.1; if (C5.Opacity < 0.2) C5.Opacity = 1;
            C6.Opacity -= 0.1; if (C6.Opacity < 0.2) C6.Opacity = 1;
            C7.Opacity -= 0.1; if (C7.Opacity < 0.2) C7.Opacity = 1;
            C8.Opacity -= 0.1; if (C8.Opacity < 0.2) C8.Opacity = 1;
        }  
  
        private void HandleLoaded(object sender, RoutedEventArgs e)
        {
            const double offset = Math.PI;
            const double step = Math.PI * 2 / 9.0;
            SetPosition(C0, offset, 0.0, step);
            SetPosition(C1, offset, 1.0, step);
            SetPosition(C2, offset, 2.0, step);
            SetPosition(C3, offset, 3.0, step);
            SetPosition(C4, offset, 4.0, step);
            SetPosition(C5, offset, 5.0, step);
            SetPosition(C6, offset, 6.0, step);
            SetPosition(C7, offset, 7.0, step);
            SetPosition(C8, offset, 8.0, step);
        }  
  
        private void SetPosition(Ellipse ellipse, double offset,  
            double posOffSet, double step)  
        {  
            ellipse.SetValue(Canvas.LeftProperty, 50.0  
                + Math.Sin(offset + posOffSet * step) * 50.0);  
  
            ellipse.SetValue(Canvas.TopProperty, 50  
                + Math.Cos(offset + posOffSet * step) * 50.0);  
        }  
  
        private void HandleUnloaded(object sender, RoutedEventArgs e)  
        {  
            Stop();  
        }  
  
        private void HandleVisibleChanged(object sender,  
            DependencyPropertyChangedEventArgs e)  
        {  
            bool isVisible = (bool)e.NewValue;  
  
            if (isVisible)  
                Start();  
            else  
                Stop();  
        }  
        #endregion     

    }
}
