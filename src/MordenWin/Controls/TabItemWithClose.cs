using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lei.UI
{
    /// <summary>
    /// TabItemWithClose.xaml 的交互逻辑
    /// </summary>
    public class TabItemWithClose : TabItem
    {
        //public TabItemWithClose()
        //{
        //    //InitializeComponent();
        //    this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("pack://application:,,,/MordenWin;component/Themes/Generic.xaml") });
        //    this.Style = (Style)FindResource("TabItemWithClose");

        //}
        public event RoutedEventHandler Close
        {
            add { AddHandler(ButtonBase.ClickEvent, value); }
            remove { RemoveHandler(ButtonBase.ClickEvent, value); }
        }

    }
}
