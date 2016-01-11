using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace Lei.UI
{
    public partial class MordenTabItemsDict
    {
        public virtual void ItemClose(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            //FrameworkElement c = (FrameworkElement)((FrameworkElement)bt.Parent).Parent;
            //TabItem item = (TabItem)System.Windows.Media.VisualTreeHelper.GetParent(c);
            //((TabControl)item.Parent).Items.Remove(item);
            TabItem item = (TabItem)bt.TemplatedParent;
            ((TabControl)item.Parent).Items.Remove(item);
        }
    }
}
