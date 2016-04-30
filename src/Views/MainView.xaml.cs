using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Lei.Common;
using Lei.UI;
using winform = System.Windows.Forms;

namespace EpubViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainView
    {
        public MainView()
        {
            InitializeComponent();
        }

        //private void TocTreeView_OnExpanded(object sender, RoutedEventArgs e)
        //{
        //    ((ItemNode)((TreeViewItem)e.OriginalSource).DataContext).Icon=null;
        //}
        private void AboutClick(object sender, RoutedEventArgs e)
        {
            about dlg = new about();
            dlg.Owner = this;
            dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            dlg.Topmost = true;
            dlg.ShowDialog();
        }
    }
}
