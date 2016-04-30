
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
using Lei.UI;

namespace EpubViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FindView : MordenWindow
    {
        public FindView()
        {
            InitializeComponent();
            Text.Focus();
        }

        private void FindView_OnDeactivated(object sender, EventArgs e)
        {
            Close();
        }

        private void FindView_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
