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
using Lei.Common;

namespace EpubHelpViewer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class about : MordenWindow
    {

        public about()
        {
            InitializeComponent();
           
        }

        private void MordenWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }       

    }
}
